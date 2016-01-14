using System.Collections.Generic;
using Octopus.Client.Model;

namespace Kraken.Services
{
    public interface IOctopusProxy
    {
        IEnumerable<EnvironmentResource> GetEnvironments();

        IEnumerable<ProjectResource> GetProjects();
    }
}