namespace Kraken.Security
{
    using System;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Kraken.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public class ApiKeyMiddleware
    {
        public ApiKeyMiddleware(RequestDelegate next, IOctopusAuthenticationProxy octopusAuthenticationProxy)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _octopusAuthenticationProxy = octopusAuthenticationProxy ?? throw new ArgumentNullException(nameof(octopusAuthenticationProxy));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            string apiKey;
            if (TryGetApiKey(httpContext.Request, out apiKey))
            {
                var userName = await _octopusAuthenticationProxy.ValidateApiKey(apiKey);
                if (!string.IsNullOrEmpty(userName))
                {
                    var principal = ClaimsPrincipalHelpers.CreatePrincipal(userName, apiKey);

                    httpContext.User = principal;
                }
            }

            await _next.Invoke(httpContext);
        }

        private static bool TryGetApiKey(HttpRequest request, out string apiKey)
        {
            StringValues headerValue;
            if (request.Headers.TryGetValue("Authorization", out headerValue))
            {
                AuthenticationHeaderValue authHeaderValue;
                if (AuthenticationHeaderValue.TryParse(headerValue, out authHeaderValue))
                {
                    apiKey = authHeaderValue.Parameter;
                    return true;
                }
            }

            if (request.Headers.TryGetValue("X-Octopus-ApiKey", out headerValue))
            {
                apiKey = headerValue;
                return true;
            }

            if (request.Headers.TryGetValue("X-NuGet-ApiKey", out headerValue))
            {
                apiKey = headerValue;
                return true;
            }

            if (request.Query.ContainsKey("apikey"))
            {
                apiKey = request.Query["apikey"];
                return true;
            }

            apiKey = null;
            return false;
        }

        private readonly RequestDelegate _next;
        private readonly IOctopusAuthenticationProxy _octopusAuthenticationProxy;
    }
}
