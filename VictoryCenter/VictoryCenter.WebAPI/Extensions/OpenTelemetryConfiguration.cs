using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
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
            .WithTracing(t => t
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:4317");
                    o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }))
            .WithMetrics(m => m
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:4317");
                    o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                }));
    }

    public static void AddOpenTelemetryLogging(this ILoggingBuilder logging)
    {
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(ServiceName, ServiceVersion);

        logging.AddOpenTelemetry(loggingOptions =>
        {
            loggingOptions.SetResourceBuilder(resourceBuilder);
            loggingOptions.IncludeScopes = true;
            loggingOptions.IncludeFormattedMessage = true;
            loggingOptions.AddOtlpExporter(o =>
            {
                o.Endpoint = new Uri("http://localhost:4317");
            });
        });
    }
}
