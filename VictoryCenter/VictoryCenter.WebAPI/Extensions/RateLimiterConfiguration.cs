using System.Threading.RateLimiting;

namespace VictoryCenter.WebAPI.Extensions;

public static class RateLimiterConfiguration
{
    public static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(RateLimitingPolicyNameConstants.SubmitContactUsForm, httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromHours(24),
                    });
            });
        });

        return services;
    }
}

public static class RateLimitingPolicyNameConstants
{
    public const string SubmitContactUsForm = "submit-contact-us-form-rate-limiting-policy";
}
