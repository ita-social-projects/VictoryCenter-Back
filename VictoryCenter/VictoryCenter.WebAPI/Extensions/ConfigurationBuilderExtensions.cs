namespace VictoryCenter.WebAPI.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder ConfigureCustom(this IConfigurationBuilder configurationBuilder, string environment)
    {
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("VICTORYCENTER_");

        return configurationBuilder;
    }

    public static ConfigurationManager AddLocalEnvironmentVariables(this ConfigurationManager configuration)
    {
        configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                                                                       ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set in configuration");
        configuration["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                                        ?? throw new InvalidOperationException("JWTOPTIONS_SECRETKEY is not set in configuration");

        configuration["JwtOptions:RefreshTokenSecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY")
                                                                    ?? throw new InvalidOperationException("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY is not set in configuration");

        configuration["PaymentSystemsConfigurations:Way4Pay:MerchantLogin"] = Environment.GetEnvironmentVariable("WAY4PAY_MERCHANT_LOGIN");
        configuration["PaymentSystemsConfigurations:Way4Pay:MerchantSecretKey"] = Environment.GetEnvironmentVariable("WAY4PAY_MERCHANT_SECRET_KEY");
        configuration["PaymentSystemsConfigurations:Way4Pay:MerchantDomainName"] = Environment.GetEnvironmentVariable("WAY4PAY_MERCHANT_DOMAIN_NAME");
        configuration["PaymentSystemsConfigurations:Way4Pay:ApiUrl"] = Environment.GetEnvironmentVariable("WAY4PAY_API_URL");

        return configuration;
    }
}
