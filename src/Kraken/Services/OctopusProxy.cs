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

    public class OctopusProxy : IOctopusProxy
    {
        public OctopusProxy(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var apiKey = httpContextAccessor.HttpContext.User.GetOctopusApiKey();

            var endpoint = new OctopusServerEndpoint(settings.Value.OctopusServerAddress, apiKey);
            _octopusRepository = new OctopusRepository(endpoint);
            _nugetRepository = PackageRepositoryFactory.Default.CreateRepository(settings.Value.NugetServerAddress);
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

        public IEnumerable<ProjectResource> GetProjects()
        {
            return _octopusRepository.Projects.FindAll();
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
                var checkDeploy = _octopusRepository.Deployments.FindOne(d => d.ReleaseId == releaseId && d.EnvironmentId == environmentId);

                if (checkDeploy != null)
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
                        var release = _octopusRepository.Releases.Get(checkDeploy.ReleaseId);

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

        public ReleaseResource CreateReleaseFromNuget(string projectId)
        {
            var project = GetProject(projectId);
            var deploymentProcess = _octopusRepository.DeploymentProcesses.Get(project.DeploymentProcessId);
            var nugetSteps = deploymentProcess.Steps.Where(s => s.Actions.Any(a => a.Properties.ContainsKey("Octopus.Action.Package.NuGetPackageId")));

            if (nugetSteps != null && nugetSteps.Any())
            {
                var selectedPackages = new List<SelectedPackage>();
                var checkRelease = _octopusRepository.Releases.FindOne(r => r.ProjectId == project.Id);

                foreach (var nugetStep in nugetSteps)
                {
                    var actions = nugetStep.Actions;
                    var nugetPackageId = actions.Select(a => a.Properties["Octopus.Action.Package.NuGetPackageId"]).FirstOrDefault();

                    // some packages are actually referenced by hashes (so a.Properties["Octopus.Action.Package.NuGetPackageId"] = "{#NugetPackage}"
                    string regexPattern = @"\#\{[a-zA-Z]+\}";
                    var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                    var match = regex.Match(nugetPackageId);
                    if (match.Success)
                    {
                        // TODO: clean up this refKey nonsense
                        var refKey = nugetPackageId.Replace("#{", "").Replace("}", "");
                        nugetPackageId = actions.Select(a => a.Properties[refKey]).FirstOrDefault();
                    }

                    var latestNugetPackage = _nugetRepository.FindPackagesById(nugetPackageId).OrderByDescending(n => n.Published).FirstOrDefault();
                    var version = latestNugetPackage.Version.ToString();

                    // assume the last deployed release is less than or equal to the latest package in nuget
                    if (checkRelease != null && checkRelease.Version == version) return checkRelease;

                    selectedPackages.Add(new SelectedPackage
                    {
                        StepName = nugetStep.Name,
                        Version = version
                    });
                }

                var release = new ReleaseResource
                {
                    ProjectId = project.Id,
                    Version = selectedPackages.First().Version,
                    ReleaseNotes = "Release created with Kraken",
                    SelectedPackages = selectedPackages
                };

                release = _octopusRepository.Releases.Create(release);
                return release;
            }
            return null;
        }

        private readonly OctopusRepository _octopusRepository;
        private readonly IPackageRepository _nugetRepository;
    }
}
