namespace Kraken.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Kraken.Security;
    using Microsoft.AspNet.Http;
    using Microsoft.Extensions.OptionsModel;
    using Octopus.Client;
    using Octopus.Client.Model;
    using NuGet;
    using System.Globalization;

    public class OctopusProxy : IOctopusProxy
    {
        public OctopusProxy(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var apiKey = httpContextAccessor.HttpContext.User.GetOctopusApiKey();

            var endpoint = new OctopusServerEndpoint(settings.Value.OctopusServerAddress, apiKey);
            _octopusRepository = new OctopusRepository(endpoint);
        }

        public IEnumerable<EnvironmentResource> GetEnvironments()
        {
            return _octopusRepository.Environments.FindAll();
        }

        public EnvironmentResource GetEnvironment(string idOrName)
        {
            return _octopusRepository.Environments.FindOne(e => e.Id == idOrName || e.Id == "Environments-" + idOrName || e.Name == idOrName);
        }

        public ProjectResource GetProject(string idOrSlugOrName)
        {
            return _octopusRepository.Projects.FindOne(p => p.Id == idOrSlugOrName || p.Id == "Projects-" + idOrSlugOrName || p.Slug == idOrSlugOrName || p.Name == idOrSlugOrName);
        }

        public IEnumerable<ProjectResource> GetProjects(string nameFilter)
        {
            return _octopusRepository.Projects.FindMany(p => string.IsNullOrEmpty(nameFilter) || CultureInfo.InvariantCulture.CompareInfo.IndexOf(p.Name, nameFilter, CompareOptions.IgnoreCase) >= 0);
        }

        public ReleaseResource GetLastestRelease(string projectId)
        {
            return _octopusRepository.Releases.FindOne(r => r.ProjectId == projectId);
        }

        public ReleaseResource GetLastDeployedRelease(string projectId, string environmentId)
        {
            var deployment = _octopusRepository.Deployments.FindOne(d => d.ProjectId == projectId && (string.IsNullOrEmpty(environmentId) || d.EnvironmentId == environmentId));
            return deployment != null ? _octopusRepository.Releases.Get(deployment.ReleaseId) : null;
        }

        public DeploymentResource DeployRelease(string releaseId, string environmentId, bool allowRedeploy = true)
        {
            var deploymentResource = new DeploymentResource
            {
                ReleaseId = releaseId,
                EnvironmentId = environmentId,
                Comments = "Deployed with Kraken"
            };

            if (!allowRedeploy)
            {
                var checkDeploy = _octopusRepository.Deployments.FindOne(d => d.EnvironmentId == environmentId);

                if (checkDeploy != null && checkDeploy.ReleaseId == releaseId)
                {
                    var task = _octopusRepository.Tasks.Get(checkDeploy.TaskId);

                    // if the task hasn't completed, don't queue up another deploy
                    if (!task.IsCompleted)
                    {
                        return null;
                    }

                    // if the task has finished successfully, only redeploy if there have been modifications made to the release since the deploy
                    if (task.FinishedSuccessfully)
                    {
                        var release = _octopusRepository.Releases.Get(releaseId);

                        // if no modifications have been made to a successful deploy since its creation, assume it's a redeploy
                        if (release.LastModifiedOn <= checkDeploy.Created)
                        {
                            return null;
                        }
                    }
                }
            }

            return _octopusRepository.Deployments.Create(deploymentResource);
        }

        public IEnumerable<DeploymentStepResource> GetNuGetDeploymentStepResources(string projectId)
        {
            var project = GetProject(projectId);
            var deploymentProcess = _octopusRepository.DeploymentProcesses.Get(project.DeploymentProcessId);
            return deploymentProcess.Steps.Where(
                s => s.Actions.Any(a => a.Properties.ContainsKey("Octopus.Action.Package.NuGetPackageId"))).ToList();
        }

        public IEnumerable<string> GetNugetPackageIdsFromSteps(IEnumerable<DeploymentStepResource> nugetSteps)
        {
            var nugetPackageIds = new List<string>();
            if (nugetSteps.Any())
            {
                foreach (var nugetStep in nugetSteps)
                {
                    var actions = nugetStep.Actions.Where(a => a.Properties.ContainsKey("Octopus.Action.Package.NuGetPackageId"));
                    nugetPackageIds.AddRange(actions.Select(GetNugetPackageIdFromAction).Where(nugetPackageId => nugetPackageId != null));
                }
            }
            return nugetPackageIds;
        }

        public ReleaseResource CreateReleases(string projectId, IEnumerable<DeploymentStepResource> steps, Dictionary<string, string> nugetPackageInfo)
        {
            var project = GetProject(projectId);
            var versioningStrategy = project.VersioningStrategy;
            string releaseVersion = null;
            var selectedPackages = new List<SelectedPackage>();

            foreach (var step in steps)
            {
                var actions = step.Actions.Where(a => a.Properties.ContainsKey("Octopus.Action.Package.NuGetPackageId"));
                if (!string.IsNullOrEmpty(versioningStrategy.DonorPackageStepId) &&
                    versioningStrategy.DonorPackageStepId == step.Id)
                {
                    var nugetPackageId = GetNugetPackageIdFromAction(actions.First());
                    releaseVersion = nugetPackageInfo[nugetPackageId];
                }

                foreach (var action in actions)
                {
                    var nugetPackageId = GetNugetPackageIdFromAction(action);
                    if (!string.IsNullOrEmpty(nugetPackageId))
                    {
                        var version = nugetPackageInfo[nugetPackageId];
                        if (!string.IsNullOrEmpty(versioningStrategy.DonorPackageStepId) &&
                            versioningStrategy.DonorPackageStepId == action.Id)
                        {
                            releaseVersion = version;
                        }
                        selectedPackages.Add(new SelectedPackage(action.Name, version));
                    }
                }
                
            }

            var release = new ReleaseResource
            {
                Version = releaseVersion,
                ProjectId = projectId,
                SelectedPackages = selectedPackages
            };

            return _octopusRepository.Releases.Create(release);
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

        private readonly OctopusRepository _octopusRepository;
    }
}
