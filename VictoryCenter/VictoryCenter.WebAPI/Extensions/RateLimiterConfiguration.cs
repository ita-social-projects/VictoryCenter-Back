using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace VictoryCenter.WebAPI.Extensions;

public static class RateLimiterConfiguration
{
    private const int ImageUploadConcurrencyPermitLimit = 2;
    private const int ImageUploadQueueLimit = 64;
    private const int DonationRequestPermitLimit = 10;
    private const int AdminLoginRequestPermitLimit = 5;

    public static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Keep aggregate decode memory bounded: at most PermitLimit images are decoded at once.
            // Extra uploads wait in the queue instead of being rejected - a queued request has not been read yet, so it holds no image data.
            options.AddConcurrencyLimiter(
                RateLimitingPolicyNameConstants.ImageUpload,
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = ImageUploadConcurrencyPermitLimit;
                    limiterOptions.QueueLimit = ImageUploadQueueLimit;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
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
            options.AddPolicy(RateLimitingPolicyNameConstants.InitiateDonation, httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = DonationRequestPermitLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    });
            });
            options.AddPolicy(RateLimitingPolicyNameConstants.AdminLogin, httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = AdminLoginRequestPermitLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    });
            });
        });

        return services;
    }
}

public static class RateLimitingPolicyNameConstants
{
    public const string SubmitContactUsForm = "submit-contact-us-form-rate-limiting-policy";
    public const string InitiateDonation = "initiate-donation-rate-limiting-policy";
    public const string AdminLogin = "admin-login-rate-limiting-policy";
    internal const string ImageUpload = "image-upload-rate-limiting-policy";
}
