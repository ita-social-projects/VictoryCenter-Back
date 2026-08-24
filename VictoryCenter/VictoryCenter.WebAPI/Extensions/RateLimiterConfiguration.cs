using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace VictoryCenter.WebAPI.Extensions;

public static class RateLimiterConfiguration
{
    private const int ImageUploadConcurrencyPermitLimit = 2;

    public static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Keep aggregate decode memory bounded without retaining rejected image bodies in a queue.
            options.AddConcurrencyLimiter(
                RateLimitingPolicyNameConstants.ImageUpload,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = ImageUploadConcurrencyPermitLimit;
                    limiterOptions.QueueLimit = 0;
                });
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
    public const string ImageUpload = "image-upload-rate-limiting-policy";
    public const string SubmitContactUsForm = "submit-contact-us-form-rate-limiting-policy";
}
