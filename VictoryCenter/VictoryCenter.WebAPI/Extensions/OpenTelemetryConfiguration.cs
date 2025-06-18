using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace VictoryCenter.WebAPI.Extensions;

public static class OpenTelemetryConfiguration
{
    private const string ServiceName = "VictoryCenter.Backend";
    private const string ServiceVersion = "1.0.0";

    public static void AddOpenTelemetryTracing(this IServiceCollection services)
    {
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(ServiceName, ServiceVersion);

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddConsoleExporter();
            });
    }

    public static void AddOpenTelemetryLogging(this ILoggingBuilder logging)
    {
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(ServiceName, ServiceVersion);

        logging.ClearProviders();
        logging.AddOpenTelemetry(loggingOptions =>
        {
            loggingOptions.SetResourceBuilder(resourceBuilder);
            loggingOptions.IncludeScopes = true;
            loggingOptions.IncludeFormattedMessage = true;
            loggingOptions.AddConsoleExporter();
        });
    }
}
