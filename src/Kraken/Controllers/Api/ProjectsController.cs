namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kraken.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Octopus.Client.Model;
    using Services;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
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
        public async Task<IEnumerable<ProjectResource>> GetProjects([FromQuery] string nameFilter = "")
        {
            return await _octopusProxy.GetProjectsAsync(nameFilter);
        }

        private readonly IOctopusProxy _octopusProxy;
    }
}