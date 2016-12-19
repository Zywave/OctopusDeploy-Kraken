namespace Kraken.TagHelpers
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    public class IETagHelper : TagHelper
    {
        public IETagHelper(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (output == null) throw new ArgumentNullException(nameof(output));

            var httpContext = _httpContextAccessor.HttpContext;
            var userAgent = httpContext.Request.Headers["User-Agent"];

            output.TagName = null;

            if (!_ieUserAgentExpression.IsMatch(userAgent))
            {
                output.SuppressOutput();
            }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly Regex _ieUserAgentExpression = new Regex(@"(?:\b(MS)?IE\s+|\bTrident\/7\.0;.*\s+rv:)(\d+)");
    }
}
