namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using Kraken.Filters;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Octopus.Client.Model;
    using Kraken.Services;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
    [Route("api/environments")]
    public class EnvironmentsController : Controller
    {
        public EnvironmentsController(IOctopusProxy octopusProxy)
        {
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _octopusProxy = octopusProxy;
        }

        [HttpGet("{permission}")]
        public IEnumerable<EnvironmentResource> GetEnvironments([FromRoute] Permission permission = Permission.EnvironmentView)
        {
            return _octopusProxy.GetEnvironments(permission == Permission.None ? Permission.EnvironmentView : permission);
        }

        private readonly IOctopusProxy _octopusProxy;
    }
}
