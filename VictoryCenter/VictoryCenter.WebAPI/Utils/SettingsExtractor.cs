using VictoryCenter.WebAPI.Utils.Settings;

namespace VictoryCenter.WebAPI.Utils;

public static class SettingsExtractor
{
    public static CorsSettings GetCorsSettings(IConfiguration configuration)
    {
        return new CorsSettings
        {
            AllowedHeaders = GetAllowedCorsValues(configuration, "AllowedHeaders"),
            AllowedMethods = GetAllowedCorsValues(configuration, "AllowedMethods"),
            AllowedOrigins = GetAllowedCorsValues(configuration, "AllowedOrigins"),
            ExposedHeaders = GetAllowedCorsValues(configuration, "ExposedHeaders"),
            PreflightMaxAge = int.TryParse(configuration.GetValue<string>("CORS:PreflightMaxAge"), out var preflightMaxAge) ? preflightMaxAge : 600
        };
    }

    private static string[] GetAllowedCorsValues(IConfiguration configuration, string key)
    {
        var allowedCorsValuesStringified = configuration.GetSection($"CORS:{key}").Get<string[]>();
        return allowedCorsValuesStringified is not null
            ? allowedCorsValuesStringified.Where(val => !string.IsNullOrWhiteSpace(val)).ToArray()
            : ["*"];
    }
}
