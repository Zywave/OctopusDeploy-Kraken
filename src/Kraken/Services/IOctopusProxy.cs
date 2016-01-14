namespace Kraken.Services
{
using System.Collections.Generic;
using Octopus.Client.Model;

    public interface IOctopusProxy
    {
        IEnumerable<EnvironmentResource> GetEnvironments();

        ProjectResource GetProject(string projectId);

         IEnumerable<ProjectResource> GetProjects();
    }
}