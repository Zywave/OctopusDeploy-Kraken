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

        public ReleaseResource GetLastestRelease(string projectId)
        {
            return _repository.Releases.FindOne(r => r.ProjectId == projectId);
        }

        public ReleaseResource GetLastDeployedRelease(string projectId, string environmentId)
        {
            var deployment = _repository.Deployments.FindOne(d => d.ProjectId == projectId && (string.IsNullOrEmpty(environmentId) || d.EnvironmentId == environmentId));
            return deployment != null ? _repository.Releases.Get(deployment.ReleaseId) : null;
        }

        public DeploymentResource DeployRelease(string releaseId, string environmentId, bool allowRedeploy = true)
        { 
            var deploymentResource = new DeploymentResource
            {
                ReleaseId = releaseId,
                EnvironmentId = environmentId,
                Comments = "Deployed with Kraken"
            };

            if (!allowRedeploy)
            {
                var checkDeploy = _repository.Deployments.FindOne(d => d.ReleaseId == releaseId && d.EnvironmentId == environmentId);
                
                if (checkDeploy != null)
                {
                    var task = _repository.Tasks.Get(checkDeploy.TaskId);
                    
                    // if the task hasn't finished successfully, then try to redeploy
                    if (task.FinishedSuccessfully)
                    {
                        var release = _repository.Releases.Get(checkDeploy.ReleaseId);

                        // if no modificatoins have been made to a successful deploy, assume it's a redeploy
                        if (release.LastModifiedOn <= checkDeploy.LastModifiedOn)
                        {
                            return null;
                        }
                    }
                }
            }

            return _repository.Deployments.Create(deploymentResource);
        }

        private readonly OctopusRepository _repository;
    }
}
