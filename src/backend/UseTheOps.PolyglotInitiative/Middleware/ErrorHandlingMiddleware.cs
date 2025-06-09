using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace UseTheOps.PolyglotInitiative.Middleware
{
    [ExcludeFromCodeCoverage]
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Détermination de la langue demandée (Accept-Language)
            var acceptLang = context.Request.Headers["Accept-Language"].ToString();
            var culture = "en-US";
            if (!string.IsNullOrEmpty(acceptLang))
            {
                try
                {
                    var parsed = acceptLang.Split(',')[0];
                    var normalized = parsed.Replace('_', '-');
                    var ci = new System.Globalization.CultureInfo(normalized);
                    System.Globalization.CultureInfo.CurrentCulture = ci;
                    System.Globalization.CultureInfo.CurrentUICulture = ci;
                    culture = normalized;
                }
                catch { /* fallback en-US */ }
            }
            else
            {
                var ci = new System.Globalization.CultureInfo("en-US");
                System.Globalization.CultureInfo.CurrentCulture = ci;
                System.Globalization.CultureInfo.CurrentUICulture = ci;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by ErrorHandlingMiddleware");

                // Construction du ProblemDetails RFC 7807
                var problem = new
                {
                    type = "https://tools.ietf.org/html/rfc7807",
                    title = UseTheOps.PolyglotInitiative.LocalizationHelper.GetString("Error_Internal"),
                    status = (int)HttpStatusCode.InternalServerError,
                    detail = UseTheOps.PolyglotInitiative.LocalizationHelper.GetString("Error_Internal"),
                    instance = context.Request.Path
                };
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));

                
            }
        }
    }
}
