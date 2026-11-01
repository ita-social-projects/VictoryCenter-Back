using Microsoft.Extensions.FileProviders;

namespace VictoryCenter.WebAPI.Extensions;

public static class ConfigureHostBuilderExtension
{
    public static PhysicalFileProvider ConfigureApplication(this ConfigureHostBuilder hostBuilder, WebApplicationBuilder appBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        try
        {
            appBuilder.Configuration.ConfigureCustom(environment, fileProvider);
            return fileProvider;
        }
        catch
        {
            fileProvider.Dispose();
            throw;
        }
    }
}
