using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.WebAPI.Extensions;

public static class ApplicationConfiguration
{
    public static void UseRequestResponseLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}