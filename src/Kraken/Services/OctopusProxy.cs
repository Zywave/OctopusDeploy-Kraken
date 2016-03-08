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
    using Octopus.Client.Exceptions;

    public class OctopusProxy : IOctopusProxy
    {
        public OctopusProxy(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var apiKey = httpContextAccessor.HttpContext.User.GetOctopusApiKey();

            var endpoint = new OctopusServerEndpoint(settings.Value.OctopusServerAddress, apiKey);
            _octopusRepository = new OctopusRepository(endpoint);
            _octopusClient = new OctopusClient(endpoint);
        }

        public IEnumerable<EnvironmentResource> GetEnvironments(Permission permission)
        {
            var environments = _octopusRepository.Environments.FindAll();

            return EnvironmentPermissionCheck(environments, permission);
        }

        public EnvironmentResource GetEnvironment(string idOrName, Permission permission)
        {
            var environment = _octopusRepository.Environments.FindOne(e => e.Id == idOrName || e.Id == "Environments-" + idOrName || string.Equals(e.Name, idOrName, StringComparison.InvariantCultureIgnoreCase));
            return EnvironmentPermissionCheck(new List<EnvironmentResource> { environment }, permission).FirstOrDefault();
        }

        public ProjectResource GetProject(string idOrSlugOrName)
        {
            return _octopusRepository.Projects.FindOne(p => p.Id == idOrSlugOrName || p.Id == "Projects-" + idOrSlugOrName || p.Slug == idOrSlugOrName || string.Equals(p.Name, idOrSlugOrName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<ProjectResource> GetProjects(string nameFilter)
        {
            return _octopusRepository.Projects.FindMany(p => string.IsNullOrEmpty(nameFilter) || CultureInfo.InvariantCulture.CompareInfo.IndexOf(p.Name, nameFilter, CompareOptions.IgnoreCase) >= 0);
        }

        public DashboardResource GetDashboardForProjectIdsAndEnvironmentIds(IEnumerable<string> projectIds, IEnumerable<string> environmentIds)
        {
            return _octopusRepository.Dashboards.GetDynamicDashboard(projectIds.ToArray(), environmentIds.ToArray());
        }

        public ReleaseResource GetLatestRelease(string projectId)
        {
            var project = GetProject(projectId);
            var releases = _octopusRepository.Projects.GetReleases(project).Items;
            return releases.FirstOrDefault();
        }

        public ReleaseResource GetLatestDeployedRelease(string projectId, string environmentId)
        {
            var deployment = _octopusRepository.Deployments.FindAll(new[] { projectId }, new[] { environmentId }).Items.FirstOrDefault();
            return deployment != null ? _octopusRepository.Releases.Get(deployment.ReleaseId) : null;
        }

        public DeploymentProcessResource GetDeploymentProcessForProject(string projectId)
        {
            var project = GetProject(projectId);
            return _octopusRepository.DeploymentProcesses.Get(project.DeploymentProcessId);
        }

        public FeedResource GetFeed(string feedId)
        {
            return _octopusRepository.Feeds.Get(feedId);
        }

        public DeploymentResource DeployRelease(string releaseId, string environmentId, bool forceRedeploy)
        {
            var deploymentResource = new DeploymentResource
            {
                ReleaseId = releaseId,
                EnvironmentId = environmentId,
                Comments = "Deployed with Kraken"
            };

            if (!forceRedeploy)
            {
                var release = _octopusRepository.Releases.Get(releaseId);
                DeploymentResource checkDeploy;
                try
                {
                    checkDeploy = _octopusRepository.Releases.GetDeployments(release).Items.FirstOrDefault(d => d.EnvironmentId == environmentId);
                }
                catch (OctopusResourceNotFoundException)
                {
                    checkDeploy = null;
                }

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

        public ReleaseResource CreateRelease(string projectId, string version, IEnumerable<SelectedPackage> selectedPackages)
        {
            var release = new ReleaseResource
            {
                Version = version,
                ProjectId = projectId,
                SelectedPackages = selectedPackages.ToList()
            };

            return CreateRelease(release);
        }

        public ReleaseResource CreateRelease(ReleaseResource release)
        {
            var project = _octopusRepository.Projects.Get(release.ProjectId);
            try
            {
                return _octopusRepository.Projects.GetReleaseByVersion(project, release.Version);
            }
            catch (OctopusResourceNotFoundException)
            {
                return _octopusRepository.Releases.Create(release);
            }
        }

        public ReleaseResource GetRelease(string projectId, string releaseVersion)
        {
            var project = _octopusRepository.Projects.Get(projectId);
            if (project != null)
            {
                try
                {
                    return _octopusRepository.Projects.GetReleaseByVersion(project, releaseVersion);
                }
                catch (OctopusResourceNotFoundException)
                {
                    return null;
                }
            }
            return null;
        }

        private IEnumerable<EnvironmentResource> EnvironmentPermissionCheck(IEnumerable<EnvironmentResource> environments, Permission permission)
        {
            var user = _octopusRepository.Users.GetCurrent();
            var permissions = _octopusRepository.Users.GetPermissions(user);
            List<UserPermissionRestriction> userPermissionRestrictions;
            var restrictedEnvironments = new List<string>();
            if (permissions.Permissions.TryGetValue(permission, out userPermissionRestrictions))
            {
                restrictedEnvironments = userPermissionRestrictions.SelectMany(p => p.RestrictedToEnvironmentIds).Distinct().ToList();
            }
            return !restrictedEnvironments.Any()
                ? environments
                : environments.Where(e => restrictedEnvironments.Contains(e.Id));
        } 

        private readonly OctopusRepository _octopusRepository;
        private readonly OctopusClient _octopusClient;
    }
}
