using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace orderflow.security.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = requestId;
            context.Response.Headers.Append("X-Correlation-ID", requestId);

            _logger.LogInformation("➡️ Request {Method} {Path} - CorrelationId: {RequestId}",
                context.Request.Method, context.Request.Path, requestId);

            await _next(context);

            _logger.LogInformation("Response {Method} {Path} - CorrelationId: {RequestId} - Status: {StatusCode}",
                context.Request.Method, context.Request.Path, requestId, context.Response.StatusCode);
        }
    }
}
