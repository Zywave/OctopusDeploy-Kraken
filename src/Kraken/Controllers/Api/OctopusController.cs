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
    [Route("api/[controller]")]
    public class OctopusController : Controller
    {
        public OctopusController(IOctopusProxy octopus)
        {
            if (octopus == null) throw new ArgumentNullException(nameof(octopus));

            _octopus = octopus;
        }

        // GET: api/Octopus/GetProjects
        [HttpGet("GetProjects")]
        public IEnumerable<ProjectResource> GetProjects()
        {
            return _octopus.GetProjects();
        }

        private readonly IOctopusProxy _octopus;
    }
}