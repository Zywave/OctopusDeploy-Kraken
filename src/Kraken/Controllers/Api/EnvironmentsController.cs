namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Octopus.Client.Model;
    using Kraken.Services;

    [Authorize]
    [Produces("application/json")]
    [Route("api/environments")]
    public class EnvironmentsController : Controller
    {
        public EnvironmentsController(IOctopusProxy octopusProxy)
        {
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _octopusProxy = octopusProxy;
        }

        [HttpGet]
        public IEnumerable<EnvironmentResource> GetEnvironments()
        {
            return _octopusProxy.GetEnvironments();
        }

        private readonly IOctopusProxy _octopusProxy;
    }
}
