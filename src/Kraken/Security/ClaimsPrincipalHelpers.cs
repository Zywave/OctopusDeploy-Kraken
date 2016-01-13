namespace Kraken.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    public static class ClaimsPrincipalHelpers
    {
        public static string GetOctopusApiKey(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));

            return claimsPrincipal.FindFirstValue(ApplicationClaims.OctopusApiKey);
        }

        public static ClaimsPrincipal CreatePrincipal(string username, string octopusApiKey)
        {
            var claims = new List<Claim>
            {
                new Claim(ApplicationClaims.UserName, username),
                new Claim(ApplicationClaims.OctopusApiKey, octopusApiKey)
            };

            var identity = new ClaimsIdentity(claims, "local", ApplicationClaims.UserName, "role");

            return new ClaimsPrincipal(identity);
        }
    }
}
