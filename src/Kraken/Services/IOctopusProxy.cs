namespace Kraken.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Octopus.Client.Model;

    public interface IOctopusProxy
    {
        Task<IEnumerable<EnvironmentResource>> GetEnvironmentsAsync();

        Task<EnvironmentResource> GetEnvironmentAsync(string idOrName);

        Task<Dictionary<Permission, IEnumerable<EnvironmentResource>>> GetEnvironmentsWithPermissionsAsync(IEnumerable<Permission> permissionsToGet, IEnumerable<string> projectIds = null);

        Task<ProjectResource> GetProjectAsync(string idOrSlugOrName);

        Task<IEnumerable<ProjectResource>> GetProjectsAsync(string nameFilter);

        Task<DashboardResource> GetDynamicDashboardAsync(IEnumerable<string> projectIds, IEnumerable<string> environmentIds);

        Task<ReleaseResource> GetLatestReleaseAsync(string projectId);

        Task<ReleaseResource> GetLatestDeployedReleaseAsync(string projectId, string environmentId);

        Task<DeploymentProcessResource> GetDeploymentProcessForProjectAsync(string projectId);

        Task<FeedResource> GetFeedAsync(string feedIdOrName);

        Task<DeploymentResource> DeployReleaseAsync(string releaseId, string environmentId, bool forceRedeploy);

        Task<ReleaseResource> CreateReleaseAsync(string projectId, string version, IEnumerable<SelectedPackage> selectedPackages);

        Task<ReleaseResource> CreateReleaseAsync(ReleaseResource release);

        Task<ReleaseResource> GetReleaseAsync(string projectId, string releaseVersion);
    }
}