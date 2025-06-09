using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UseTheOps.PolyglotInitiative.Middleware
{
    /// <summary>
    /// Middleware for OpenTelemetry activity propagation and tracing.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OpenTelemetryActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OpenTelemetryActivityMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTelemetryActivityMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">The logger.</param>
        public OpenTelemetryActivityMiddleware(RequestDelegate next, ILogger<OpenTelemetryActivityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
