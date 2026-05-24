namespace VictoryCenter.WebAPI.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder ConfigureCustom(
        this IConfigurationBuilder configurationBuilder,
        string environment)
    {
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables("VICTORYCENTER_");

        return configurationBuilder;
    }

    public static IConfiguration AddLocalEnvironmentVariables(this IConfiguration configuration)
    {
        configuration["ConnectionStrings:DefaultConnection"] =
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set in configuration");
        configuration["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                                ?? throw new InvalidOperationException(
                                                    "JWTOPTIONS_SECRETKEY is not set in configuration");

        configuration["JwtOptions:RefreshTokenSecretKey"] =
            Environment.GetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY")
            ?? throw new InvalidOperationException("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY is not set in configuration");

        configuration["PaymentSystemsConfigurations:Way4Pay:MerchantLogin"] =
            Environment.GetEnvironmentVariable("WAY4PAY_MERCHANT_LOGIN");
        configuration["PaymentSystemsConfigurations:Way4Pay:MerchantSecretKey"] =
            Environment.GetEnvironmentVariable("WAY4PAY_MERCHANT_SECRET_KEY");
        configuration["PaymentSystemsConfigurations:Way4Pay:MerchantDomainName"] =
            Environment.GetEnvironmentVariable("WAY4PAY_MERCHANT_DOMAIN_NAME");
        configuration["PaymentSystemsConfigurations:Way4Pay:ApiUrl"] =
            Environment.GetEnvironmentVariable("WAY4PAY_API_URL");
        configuration["BlobEnvironmentVariables:Local:BlobStoreKey"] =
            Environment.GetEnvironmentVariable("BLOB_LOCAL_STORE_KEY")
            ?? throw new InvalidOperationException("BLOB_LOCAL_STORE_KEY is not set in environment variables");

        configuration["CloudflareTurnstileCaptchaOptions:SecretKey"] =
            Environment.GetEnvironmentVariable("CLOUDFLARE_TURNSTILE_SECRET_KEY")
            ?? throw new InvalidOperationException(
                "CLOUDFLARE_TURNSTILE_SECRET_KEY is not set in environment variables");
        configuration["CloudflareTurnstileCaptchaOptions:SiteVerifyUrl"] =
            Environment.GetEnvironmentVariable("CLOUDFLARE_TURNSTILE_SITE_VERIFY_URL")
            ?? throw new InvalidOperationException(
                "CLOUDFLARE_TURNSTILE_SITE_VERIFY_URL is not set in environment variables");

        configuration["EmailOptions:ResendApiToken"] = Environment.GetEnvironmentVariable("RESEND_API_TOKEN");

        configuration["ContactUsEmailOptions:FromAddress"] =
            Environment.GetEnvironmentVariable("CONTACT_US_EMAIL_OPTIONS_FROM_ADDRESS")
            ?? throw new InvalidOperationException(
                "CONTACT_US_EMAIL_OPTIONS_FROM_ADDRESS is not set in environment variables");

        return configuration;
    }
}
