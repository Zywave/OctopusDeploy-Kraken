namespace Kraken.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Octopus.Client.Model;

    public class OctopusReleaseService : IOctopusReleaseService
    {
        public OctopusReleaseService(IOctopusProxy octopusProxy, INuGetProxy nuGetProxy)
        {
            _octopusProxy = octopusProxy;
            _nuGetProxy = nuGetProxy;
        }

        public ReleaseResource CreateRelease(string projectId, string version = null)
        {
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
                    var nugetPackageId = _octopusProxy.GetNugetPackageIdFromAction(actions.First());
                    version = _nuGetProxy.GetLatestVersionForPackage(nugetPackageId);
                }

                foreach (var action in actions)
                {
                    var nugetPackageId = _octopusProxy.GetNugetPackageIdFromAction(action);
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

            return _octopusProxy.CreateRelease(projectId, version, selectedPackages);
        }

        private readonly IOctopusProxy _octopusProxy;
        private readonly INuGetProxy _nuGetProxy;
    }
}
