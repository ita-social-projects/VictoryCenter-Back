namespace VictoryCenter.WebAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly IRequestLogSanitizer _sanitizer;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        IRequestLogSanitizer sanitizer)
    {
        _next = next;
        _logger = logger;
        _sanitizer = sanitizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(
            state =>
            {
                var httpContext = (HttpContext)state;
                var status = httpContext.Response.StatusCode;

                var level = status switch
                {
                    >= 500 => LogLevel.Error,
                    >= 400 => LogLevel.Warning,
                    >= 200 => LogLevel.Information,
                    _ => LogLevel.Debug
                };

                _logger.Log(
                    level,
                    "HTTP {Method} {Path} got responded with {StatusCode}",
                    httpContext.Request.Method,
                    _sanitizer.GetSafePathAndQuery(httpContext.Request),
                    status);

                return Task.CompletedTask;
            }, context);

        await _next(context);
    }
}
