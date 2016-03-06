namespace Kraken.Services
{
    using System.Collections.Generic;
    using Octopus.Client.Model;

    public interface IOctopusProxy
    {
        IEnumerable<EnvironmentResource> GetEnvironments();

        EnvironmentResource GetEnvironment(string idOrName);

        ProjectResource GetProject(string idOrSlugOrName);

        IEnumerable<ProjectResource> GetProjects(string nameFilter);

        ReleaseResource GetLatestRelease(string projectId);

        ReleaseResource GetLatestDeployedRelease(string projectId, string environmentId);

        DeploymentProcessResource GetDeploymentProcessForProject(string projectId);

        FeedResource GetFeed(string feedIdOrName);

        DeploymentResource DeployRelease(string releaseId, string environmentId, bool forceRedeploy);

        ReleaseResource CreateRelease(string projectId, string version, IEnumerable<SelectedPackage> selectedPackages);

        ReleaseResource CreateRelease(ReleaseResource release);
    }
}