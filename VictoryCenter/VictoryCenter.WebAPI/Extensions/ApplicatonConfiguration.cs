using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.WebAPI.Extensions;

public static class ApplicationConfiguration
{
    public static void UseErrorLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<ErrorLoggingMiddleware>();
    }
}