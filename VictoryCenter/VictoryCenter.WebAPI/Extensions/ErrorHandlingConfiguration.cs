using VictoryCenter.WebAPI.ErrorHandling;

namespace VictoryCenter.WebAPI.Extensions;

public static class ErrorHandlingConfiguration
{
    public static IServiceCollection AddApiExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();

        // Keep the application's sanitized log; avoid a second framework exception log on .NET 9.
        services.AddLogging(logging => logging.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None));
#if NET10_0_OR_GREATER
        // Preserve the .NET 9 diagnostics/metrics behavior when the runtime is upgraded.
        services.Configure<ExceptionHandlerOptions>(options => options.SuppressDiagnosticsCallback = _ => false);
#endif
        return services;
    }
}
