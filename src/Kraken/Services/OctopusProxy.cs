namespace Kraken.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kraken.Security;
    using Microsoft.AspNet.Http;
    using Microsoft.Extensions.OptionsModel;
    using Octopus.Client;
    using Octopus.Client.Model;

    public class OctopusProxy : IOctopusProxy
    {
        public OctopusProxy(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var apiKey = httpContextAccessor.HttpContext.User.GetOctopusApiKey();
            
            var endpoint = new OctopusServerEndpoint(settings.Value.OctopusServerAddress, apiKey);
            _repository = new OctopusRepository(endpoint);
        }

        public IEnumerable<EnvironmentResource> GetEnvironments()
        {
            return _repository.Environments.FindAll();
        }
        
        public ProjectResource GetProject(string projectId)
        {
            return _repository.Projects.Get(projectId);
        }

        public IEnumerable<ProjectResource> GetProjects()
        {
            return _repository.Projects.FindAll();
        }

        public ReleaseResource GetLastDeployedReleaseForProjectAndEnvironment(string projectId, string environmentId)
        {
            var deployment =
                _repository.Deployments.FindAll(new[] { projectId }, new[] { environmentId })
                    .Items.OrderByDescending(d => d.Created).FirstOrDefault();
            return deployment != null ? _repository.Releases.Get(deployment.ReleaseId) : null;
        }

        private readonly OctopusRepository _repository;
    }
}
