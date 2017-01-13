namespace Kraken.Filters
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;

    public class ResponseTextExceptionFilter : IExceptionFilter
    {
        public ResponseTextExceptionFilter(IOptions<AppSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            IncludeExceptionDetails = settings.Value.IncludeExceptionDetailsInResponseText;
        }

        public bool IncludeExceptionDetails { get; set; }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            context.Result = new ContentResult
            {
                Content = IncludeExceptionDetails ? exception.ToString() : String.Empty,
                StatusCode = 500
            };
        }
    }
}
