namespace Kraken.Services
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
                foreach (var action in actions)
                {
                    var nuGetPackageId = GetNuGetPackageIdFromAction(action);
                    if (!string.IsNullOrEmpty(nuGetPackageId))
                    {
                        var feedId = GetNuGetFeedIdFromAction(action);
                        var feed = _octopusProxy.GetFeed(feedId);
                        var nuGetPackageVersion = _nuGetProxy.GetLatestVersionForPackage(nuGetPackageId, feed.FeedUri);
                        if (string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(versioningStrategy.DonorPackageStepId) &&
                            versioningStrategy.DonorPackageStepId == action.Id)
                        {
                            version = nuGetPackageVersion;
                        }
                        selectedPackages.Add(new SelectedPackage(action.Name, nuGetPackageVersion));
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

        private static string GetNuGetPackageIdFromAction(DeploymentActionResource action) 
        {
            PropertyValueResource nuGetPackageId;
            if (action.Properties.TryGetValue("Octopus.Action.Package.NuGetPackageId", out nuGetPackageId))
            {
                // some packages are actually referenced by hashes (so a.Properties["Octopus.Action.Package.NuGetPackageId"] = "{#NuGetPackage}")
                var regexPattern = @"\#\{[a-zA-Z]+\}";
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                var match = regex.Match(nuGetPackageId.Value);
                if (match.Success)
                {
                    // TODO: clean up this refKey nonsense
                    var refKey = nuGetPackageId.Value.Replace("#{", "").Replace("}", "");
                    nuGetPackageId = action.Properties[refKey];
                }
            }
            return nuGetPackageId.Value;
        }

        private static string GetNuGetFeedIdFromAction(DeploymentActionResource action)
        {
            PropertyValueResource nuGetFeed;
            return action.Properties.TryGetValue("Octopus.Action.Package.NuGetFeedId", out nuGetFeed) ? nuGetFeed.Value : null;
        }

        private readonly IOctopusProxy _octopusProxy;
        private readonly INuGetProxy _nuGetProxy;
    }
}
