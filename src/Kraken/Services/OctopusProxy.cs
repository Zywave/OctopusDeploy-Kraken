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

        public EnvironmentResource GetEnvironment(string idOrName)
        {
            return _repository.Environments.FindOne(e => e.Id == idOrName || e.Id == "Environments-" + idOrName || e.Name == idOrName);
        }

        public ProjectResource GetProject(string idOrSlugOrName)
        {
            return _repository.Projects.FindOne(p => p.Id == idOrSlugOrName || p.Id == "Projects-" + idOrSlugOrName || p.Slug == idOrSlugOrName || p.Name == idOrSlugOrName);
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
                    
                    // if the task hasn't completed, don't queue up another deploy
                    if (!task.IsCompleted)
                    {
                        return null;
                    }

                    // if the task has finished successfully, only redeploy if there have been modifications made to the release since the deploy
                    if (task.FinishedSuccessfully)
                    {
                        var release = _repository.Releases.Get(checkDeploy.ReleaseId);

                        // if no modifications have been made to a successful deploy since its creation, assume it's a redeploy
                        if (release.LastModifiedOn <= checkDeploy.Created)
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
