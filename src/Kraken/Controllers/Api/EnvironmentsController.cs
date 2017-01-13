namespace Kraken.Controllers.Api
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Kraken.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IEnumerable<EnvironmentResource>> GetEnvironments()
        {
            return await _octopusProxy.GetEnvironmentsAsync();
        }

        private readonly IOctopusProxy _octopusProxy;
    }
}
