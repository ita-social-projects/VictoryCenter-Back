namespace VictoryCenter.WebAPI.Middleware;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;

    public ErrorLoggingMiddleware(RequestDelegate next,
                                ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext)state;

            if (httpContext.Response.StatusCode >= 400)
            {
                _logger.LogError(
                    "HTTP {Method} {Path} got responend with {StatusCode}",
                    httpContext.Request.Method,
                    httpContext.Request.Path + httpContext.Request.QueryString,
                    httpContext.Response.StatusCode
                );
            }
            return Task.CompletedTask;
        }, context);

        await _next(context);
    }
}