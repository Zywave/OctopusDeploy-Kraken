namespace Kraken.Services
{
using System.Collections.Generic;
using Octopus.Client.Model;

    public interface IOctopusProxy
    {
        IEnumerable<EnvironmentResource> GetEnvironments();

        EnvironmentResource GetEnvironment(string idOrName);

        ProjectResource GetProject(string idOrSlugOrName);

        IEnumerable<ProjectResource> GetProjects(string searchQuery);

        ReleaseResource GetLastestRelease(string projectId);

        ReleaseResource GetLastDeployedRelease(string projectId, string environmentId);

        DeploymentResource DeployRelease(string releaseId, string environmentId, bool allowRedploy = true);

        IEnumerable<DeploymentStepResource> GetNuGetDeploymentStepResources(string projectId);

        IEnumerable<string> GetNugetPackageIdsFromSteps(IEnumerable<DeploymentStepResource> nugetSteps);

        ReleaseResource CreateRelease(string projectId, IEnumerable<DeploymentStepResource> steps, Dictionary<string, string> nugetPackageInfo, string releaseVersion = null);
    }
}