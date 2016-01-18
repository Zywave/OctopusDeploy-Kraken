namespace Kraken.Services
{
using System.Collections.Generic;
using Octopus.Client.Model;

    public interface IOctopusProxy
    {
        IEnumerable<EnvironmentResource> GetEnvironments();

        EnvironmentResource GetEnvironment(string idOrName);

        ProjectResource GetProject(string idOrName);

        IEnumerable<ProjectResource> GetProjects();

        ReleaseResource GetLastestRelease(string projectId);

        ReleaseResource GetLastDeployedRelease(string projectId, string environmentId);

        DeploymentResource DeployRelease(string releaseId, string environmentId, bool allowRedploy = true);
    }
}