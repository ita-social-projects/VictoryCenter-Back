namespace VictoryCenter.WebAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(
            state =>
        {
            var httpContext = (HttpContext)state;
            var status = httpContext.Response.StatusCode;

            LogLevel level = status switch
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
                httpContext.Request.Path + httpContext.Request.QueryString,
                status);

            return Task.CompletedTask;
        }, context);

        await _next(context);
    }
}
