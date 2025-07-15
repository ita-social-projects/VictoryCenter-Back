namespace VictoryCenter.WebAPI.Extensions;

public static class ConfigureHostBuilderExtension
{
    public static void ConfigureApplication(this ConfigureHostBuilder hostBuilder, WebApplicationBuilder appBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        appBuilder.Configuration.ConfigureCustom(environment);
    }
}
