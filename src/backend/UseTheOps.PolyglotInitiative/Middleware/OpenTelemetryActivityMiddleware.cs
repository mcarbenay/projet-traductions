using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UseTheOps.PolyglotInitiative.Middleware
{
    [ExcludeFromCodeCoverage]
    public class OpenTelemetryActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OpenTelemetryActivityMiddleware> _logger;

        public OpenTelemetryActivityMiddleware(RequestDelegate next, ILogger<OpenTelemetryActivityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            using var activity = new Activity($"HTTP {context.Request.Method} {context.Request.Path}").Start();
            try
            {
                await _next(context);
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
