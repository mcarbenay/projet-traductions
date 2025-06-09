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
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";
                var problem = new
                {
                    type = "about:blank",
                    title = LocalizationHelper.GetString("Error_Internal"),
                    status = 500,
                    detail = LocalizationHelper.GetString("Error_Internal")
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}
