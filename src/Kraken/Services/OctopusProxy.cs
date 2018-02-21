namespace Kraken.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Octopus.Client;
    using Octopus.Client.Model;
    using System.Globalization;
    using System.Threading.Tasks;
    using Kraken.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Octopus.Client.Exceptions;

    public class OctopusProxy : IOctopusProxy, IDisposable
    {
        public OctopusProxy(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var apiKey = httpContextAccessor.HttpContext.User.GetOctopusApiKey();

            _endpoint = new OctopusServerEndpoint(settings.Value.OctopusServerAddress, apiKey);
        }

        public async Task<IEnumerable<EnvironmentResource>> GetEnvironmentsAsync()
        {
            await InitializeClient();
            return await _octopusClient.Repository.Environments.FindAll();
        }

        public async Task<EnvironmentResource> GetEnvironmentAsync(string idOrName)
        {
            await InitializeClient();
            return await _octopusClient.Repository.Environments.FindOne(e => e.Id == idOrName || e.Id == "Environments-" + idOrName || string.Equals(e.Name, idOrName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Dictionary<Permission, IEnumerable<EnvironmentResource>>> GetEnvironmentsWithPermissionsAsync(IEnumerable<Permission> permissionsToGet, IEnumerable<string> projectIds = null)
        {
            await InitializeClient();

            if (projectIds == null)
            {
                projectIds = new List<string>();
            }

            var environmentsWithPermissions = new Dictionary<Permission, IEnumerable<EnvironmentResource>>();

            var environments = await _octopusClient.Repository.Environments.FindAll();
            var user = await _octopusClient.Repository.Users.GetCurrent();
            var permissions = await _octopusClient.Repository.Users.GetPermissions(user);

            foreach (var permissionToGet in permissionsToGet)
            {
                List<UserPermissionRestriction> userPermissionRestrictions;
                var restrictedEnvironments = new List<string>();
                if (permissions.Permissions.TryGetValue(permissionToGet, out userPermissionRestrictions))
                {
                    foreach (var userPermissionRestriction in userPermissionRestrictions)
                    {
                        if (!userPermissionRestriction.RestrictedToProjectIds.Any() ||
                            userPermissionRestriction.RestrictedToProjectIds.Intersect(projectIds).Any())
                        {
                            // user has access to ALL environments for a project passed in
                            if (!userPermissionRestriction.RestrictedToEnvironmentIds.Any())
                            {
                                restrictedEnvironments.AddRange(environments.Select(e => e.Id));
                                break;
                            }
                            restrictedEnvironments.AddRange(userPermissionRestriction.RestrictedToEnvironmentIds);
                        }
                    }
                }
                environmentsWithPermissions.Add(permissionToGet,
                    !restrictedEnvironments.Any()
                        ? environments
                        : environments.Where(e => restrictedEnvironments.Contains(e.Id)));
            }

            return environmentsWithPermissions;
        }

        public async Task<ProjectResource> GetProjectAsync(string idOrSlugOrName)
        {
            await InitializeClient();

            return await _octopusClient.Repository.Projects.FindOne(p => p.Id == idOrSlugOrName || p.Id == "Projects-" + idOrSlugOrName || p.Slug == idOrSlugOrName || string.Equals(p.Name, idOrSlugOrName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<ProjectResource>> GetProjectsAsync(string nameFilter)
        {
            await InitializeClient();

            return await _octopusClient.Repository.Projects.FindMany(p => string.IsNullOrEmpty(nameFilter) || CultureInfo.InvariantCulture.CompareInfo.IndexOf(p.Name, nameFilter, CompareOptions.IgnoreCase) >= 0);
        }

        public async Task<DashboardResource> GetDynamicDashboardAsync(IEnumerable<string> projectIds, IEnumerable<string> environmentIds)
        {
            await InitializeClient();

            return await _octopusClient.Repository.Dashboards.GetDynamicDashboard(projectIds.ToArray(), environmentIds.ToArray());
        }

        public async Task<ReleaseResource> GetLatestReleaseAsync(string projectId)
        {
            await InitializeClient();

            var project = await GetProjectAsync(projectId);
            var releases = (await _octopusClient.Repository.Projects.GetReleases(project)).Items;
            return releases.FirstOrDefault();
        }

        public async Task<ReleaseResource> GetLatestDeployedReleaseAsync(string projectId, string environmentId)
        {
            await InitializeClient();

            var deployment = (await _octopusClient.Repository.Deployments.FindBy(new[] { projectId }, new[] { environmentId })).Items.FirstOrDefault();
            return deployment != null ? await _octopusClient.Repository.Releases.Get(deployment.ReleaseId) : null;
        }

        public async Task<DeploymentProcessResource> GetDeploymentProcessForProjectAsync(string projectId)
        {
            await InitializeClient();

            var project = await GetProjectAsync(projectId);
            return await _octopusClient.Repository.DeploymentProcesses.Get(project.DeploymentProcessId);
        }

        public async Task<FeedResource> GetFeedAsync(string feedId)
        {
            await InitializeClient();

            return await _octopusClient.Repository.Feeds.Get(feedId);
        }

        public async Task<DeploymentResource> DeployReleaseAsync(string releaseId, string environmentId, bool forceRedeploy)
        {
            await InitializeClient();

            var deploymentResource = new DeploymentResource
            {
                ReleaseId = releaseId,
                EnvironmentId = environmentId,
                Comments = "Deployed with Kraken"
            };

            if (!forceRedeploy)
            {
                var release = await _octopusClient.Repository.Releases.Get(releaseId);
                DeploymentResource checkDeploy;
                try
                {
                    checkDeploy = (await _octopusClient.Repository.Releases.GetDeployments(release)).Items.FirstOrDefault(d => d.EnvironmentId == environmentId);
                }
                catch (OctopusResourceNotFoundException)
                {
                    checkDeploy = null;
                }

                if (checkDeploy != null && checkDeploy.ReleaseId == releaseId)
                {
                    var task = await _octopusClient.Repository.Tasks.Get(checkDeploy.TaskId);

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

            return await _octopusClient.Repository.Deployments.Create(deploymentResource);
        }

        public async Task<ReleaseResource> CreateReleaseAsync(string projectId, string version, IEnumerable<SelectedPackage> selectedPackages)
        {
            await InitializeClient();

            var release = new ReleaseResource
            {
                Version = version,
                ProjectId = projectId,
                SelectedPackages = selectedPackages.ToList()
            };

            return await CreateReleaseAsync(release);
        }

        public async Task<ReleaseResource> CreateReleaseAsync(ReleaseResource release)
        {
            await InitializeClient();

            var project = await _octopusClient.Repository.Projects.Get(release.ProjectId);
            try
            {
                return await _octopusClient.Repository.Projects.GetReleaseByVersion(project, release.Version);
            }
            catch (OctopusResourceNotFoundException)
            {
                return await _octopusClient.Repository.Releases.Create(release);
            }
        }

        public async Task<ReleaseResource> GetReleaseAsync(string projectId, string releaseVersion)
        {
            await InitializeClient();

            var project = await _octopusClient.Repository.Projects.Get(projectId);
            if (project != null)
            {
                try
                {
                    return await _octopusClient.Repository.Projects.GetReleaseByVersion(project, releaseVersion);
                }
                catch (OctopusResourceNotFoundException)
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<VariableSetResource> GetVariableSetForProject(string projectId)
        {
            await InitializeClient();

            var project = await GetProjectAsync(projectId);
            return await _octopusClient.Repository.VariableSets.Get(project.VariableSetId);
        }

        public void Dispose()
        {
            _octopusClient?.Dispose();
        }

        private async Task InitializeClient()
        {
            if (_octopusClient == null)
            {
                _octopusClient = await OctopusAsyncClient.Create(_endpoint, new OctopusClientOptions());
            }
        }

        private readonly OctopusServerEndpoint _endpoint;
        private IOctopusAsyncClient _octopusClient;
    }
}
