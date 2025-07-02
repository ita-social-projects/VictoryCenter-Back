namespace VictoryCenter.WebAPI.Utils.Settings;

public class CorsSettings
{
    public string[] AllowedOrigins { get; init; } = null!;
    public string[] AllowedHeaders { get; init; } = null!;
    public string[] AllowedMethods { get; init; } = null!;
    public string[] ExposedHeaders { get; init; } = null!;
    public int PreflightMaxAge { get; init; }
}
