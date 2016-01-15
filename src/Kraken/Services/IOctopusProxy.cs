namespace Kraken.Services
{
using System.Collections.Generic;
using Octopus.Client.Model;

    public interface IOctopusProxy
    {
        IEnumerable<EnvironmentResource> GetEnvironments();

        ProjectResource GetProject(string projectId);

        IEnumerable<ProjectResource> GetProjects();

        ReleaseResource GetLastestRelease(string projectId);

        ReleaseResource GetLastDeployedRelease(string projectId, string environmentId);
    }
}