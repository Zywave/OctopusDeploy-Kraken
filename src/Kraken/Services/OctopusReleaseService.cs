namespace Kraken.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Octopus.Client.Model;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class OctopusReleaseService : IOctopusReleaseService
    {
        public OctopusReleaseService(IOctopusProxy octopusProxy, INuGetProxy nuGetProxy)
        {
            _octopusProxy = octopusProxy;
            _nuGetProxy = nuGetProxy;
        }

        public async Task<ReleaseResource> GetNextReleaseAsync(string projectId)
        {
            string version = null;
            var project = await _octopusProxy.GetProjectAsync(projectId);
            var versioningStrategy = project.VersioningStrategy;
            var deploymentProcess = await _octopusProxy.GetDeploymentProcessForProjectAsync(projectId);

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
                        var feed = await _octopusProxy.GetFeedAsync(feedId);
                        var nuGetPackageVersion = await _nuGetProxy.GetLatestVersionForPackageAsync(nuGetPackageId, feed.FeedUri);
                        if (string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(versioningStrategy.DonorPackageStepId) &&
                            versioningStrategy.DonorPackageStepId == action.Id)
                        {
                            version = nuGetPackageVersion.ToString();
                        }
                        selectedPackages.Add(new SelectedPackage(action.Name, nuGetPackageVersion.ToString()));
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
                const string regexPattern = @"\#\{[a-zA-Z]+\}";
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                var match = regex.Match(nuGetPackageId.Value);
                if (match.Success)
                {
                    var refKey = nuGetPackageId.Value.Replace("#{", "").Replace("}", "");
                    nuGetPackageId = action.Properties[refKey];
                }
            }
            return nuGetPackageId?.Value;
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
