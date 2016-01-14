namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Kraken.Models;
    using Microsoft.AspNet.Authorization;
    using Octopus.Client.Model;
    using Services;
    [Authorize]
    [Produces("application/json")]
    [Route("api/releasebatches")]
    public class OctopusController : Controller
    {
        public OctopusController(OctopusProxy octopus)
        {
            if (octopus == null) throw new ArgumentNullException(nameof(octopus));

            _octopus = octopus;
        }

        // GET: api/Octopus/GetProjects
        [HttpGet]
        public IEnumerable<ProjectResource> GetProjects()
        {
            return _octopus.GetProjects();
        }

        private readonly OctopusProxy _octopus;
    }
}