﻿namespace Kraken.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Octopus.Client.Model;
    using System.Text.RegularExpressions;

    public class OctopusReleaseService : IOctopusReleaseService
    {
        public OctopusReleaseService(IOctopusProxy octopusProxy, INuGetProxy nuGetProxy)
        {
            _octopusProxy = octopusProxy;
            _nuGetProxy = nuGetProxy;
        }

        public ReleaseResource GetNextRelease(string projectId)
        {
            string version = null;
            var project = _octopusProxy.GetProject(projectId);
            var versioningStrategy = project.VersioningStrategy;
            var deploymentProcess = _octopusProxy.GetDeploymentProcessForProject(projectId);

            var selectedPackages = new List<SelectedPackage>();

            foreach (var step in deploymentProcess.Steps)
            {
                var actions = step.Actions.Where(a => a.Properties.ContainsKey("Octopus.Action.Package.NuGetPackageId")).ToList();
                if (string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(versioningStrategy.DonorPackageStepId) &&
                    versioningStrategy.DonorPackageStepId == step.Id)
                {
                    var nugetPackageId = GetNugetPackageIdFromAction(actions.First());
                    version = _nuGetProxy.GetLatestVersionForPackage(nugetPackageId);
                }

                foreach (var action in actions)
                {
                    var nugetPackageId = GetNugetPackageIdFromAction(action);
                    if (!string.IsNullOrEmpty(nugetPackageId))
                    {
                        var nugetPackageVersion = _nuGetProxy.GetLatestVersionForPackage(nugetPackageId);
                        if (string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(versioningStrategy.DonorPackageStepId) &&
                            versioningStrategy.DonorPackageStepId == action.Id)
                        {
                            version = nugetPackageVersion;
                        }
                        selectedPackages.Add(new SelectedPackage(action.Name, version));
                    }
                }

            }

            return new ReleaseResource
            {
                ProjectId = projectId,
                Version = version,
                SelectedPackages = selectedPackages
            };
        }

        private string GetNugetPackageIdFromAction(DeploymentActionResource action)
        {
            string nugetPackageId;
            if (action.Properties.TryGetValue("Octopus.Action.Package.NuGetPackageId", out nugetPackageId))
            {
                // some packages are actually referenced by hashes (so a.Properties["Octopus.Action.Package.NuGetPackageId"] = "{#NugetPackage}"
                string regexPattern = @"\#\{[a-zA-Z]+\}";
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                var match = regex.Match(nugetPackageId);
                if (match.Success)
                {
                    // TODO: clean up this refKey nonsense
                    var refKey = nugetPackageId.Replace("#{", "").Replace("}", "");
                    nugetPackageId = action.Properties[refKey];
                }
                return nugetPackageId;
            }
            return null;
        }

        private readonly IOctopusProxy _octopusProxy;
        private readonly INuGetProxy _nuGetProxy;
    }
}