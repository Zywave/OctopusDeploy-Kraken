namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Octopus.Client.Model;
    using Services;

    [Authorize]
    [Produces("application/json")]
    [Route("api/projects")]
    public class ProjectsController : Controller
    {
        public ProjectsController(IOctopusProxy octopusProxy)
        {
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _octopusProxy = octopusProxy;
        }

        // GET: api/projects
        [HttpGet]
        public IEnumerable<ProjectResource> GetProjects()
        {
            return _octopusProxy.GetProjects();
        }

        private readonly IOctopusProxy _octopusProxy;
    }
}