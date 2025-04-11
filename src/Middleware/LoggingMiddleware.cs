namespace orderflow.security.Middleware;

/// <summary>
/// Middleware that logs HTTP request and response details along with a unique Correlation ID for tracing.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger to log the request and response details.</param>
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to log the request and response details, including a unique Correlation ID.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        // Generate a unique CorrelationId for this request
        var requestId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = requestId;
        context.Response.Headers.Append("X-Correlation-ID", requestId);

        // Log request details
        _logger.LogInformation("➡️ Request {Method} {Path} - CorrelationId: {RequestId}",
            context.Request.Method, context.Request.Path, requestId);

        // Call the next middleware in the pipeline
        await _next(context);

        // Log response details
        _logger.LogInformation("Response {Method} {Path} - CorrelationId: {RequestId} - Status: {StatusCode}",
            context.Request.Method, context.Request.Path, requestId, context.Response.StatusCode);
    }
}

