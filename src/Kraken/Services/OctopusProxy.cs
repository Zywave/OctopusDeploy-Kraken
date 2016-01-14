namespace Kraken.Services
{
    using System;
    using System.Collections.Generic;
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

        private readonly OctopusRepository _repository;
    }
}
