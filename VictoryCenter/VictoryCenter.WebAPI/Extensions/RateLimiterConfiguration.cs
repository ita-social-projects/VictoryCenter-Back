using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace VictoryCenter.WebAPI.Extensions;

public static class RateLimiterConfiguration
{
    private const string UnknownClientPartitionKey = "unknown";
    private const int IPv6PrefixLengthInBytes = 8;

    public static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services)
    {
        services.AddOptions<RateLimitingOptions>()
            .BindConfiguration(RateLimitingOptions.SectionName)
            .ValidateOnStart();
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<RateLimitingOptions>, RateLimitingOptionsValidator>());

        services.AddRateLimiter(_ => { });

        // Configured through IOptions (not eagerly from IConfiguration), so configuration added after
        // service registration, e.g. by WebApplicationFactory, still applies.
        services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimitingOptions>>((options, limits) => Configure(options, limits.Value));

        return services;
    }

    /// <summary>
    /// Returns the rate-limit partition key for the client that sent the request.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="ConnectionInfo.RemoteIpAddress"/> only. Behind a reverse proxy, the forwarded-headers
    /// middleware sets it from <c>X-Forwarded-For</c> only when the proxy is in <c>ReverseProxy:KnownNetworks</c>,
    /// so clients can't choose their own partition by spoofing headers.
    /// IPv4-mapped IPv6 addresses are treated as IPv4. Other IPv6 addresses are grouped by /64, because a single
    /// client usually controls a whole /64 and could otherwise rotate addresses to get fresh limits.
    /// </remarks>
    /// <param name="httpContext">The current request.</param>
    /// <returns>The client IPv4 address, the client IPv6 /64 prefix, or <c>"unknown"</c>.</returns>
    public static string GetClientPartitionKey(HttpContext httpContext)
    {
        var address = httpContext.Connection.RemoteIpAddress;

        if (address is null)
        {
            return UnknownClientPartitionKey;
        }

        if (address.IsIPv4MappedToIPv6)
        {
            return address.MapToIPv4().ToString();
        }

        if (address.AddressFamily != AddressFamily.InterNetworkV6)
        {
            return address.ToString();
        }

        Span<byte> bytes = stackalloc byte[16];
        address.TryWriteBytes(bytes, out _);
        bytes[IPv6PrefixLengthInBytes..].Clear();

        return $"{new IPAddress(bytes)}/64";
    }

    private static void Configure(RateLimiterOptions options, RateLimitingOptions limits)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = RateLimitRejectionHandler.OnRejectedAsync;

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
            context =>
            {
                var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

                if (env.IsDevelopment() ||
                    !context.Request.Path.StartsWithSegments("/swagger"))
                {
                    return RateLimitPartition.GetNoLimiter("non-swagger");
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"swagger:{GetClientPartitionKey(context)}",
                    factory: _ => CreateFixedWindowOptions(limits.Swagger));
            });

        // Keep aggregate decode memory bounded: at most PermitLimit images are decoded at once.
        // Extra uploads wait in the queue instead of being rejected - a queued request has not been read yet, so it holds no image data.
        options.AddConcurrencyLimiter(
            RateLimitingPolicyNameConstants.ImageUpload,
            limiterOptions =>
            {
                limiterOptions.PermitLimit = limits.ImageUpload.PermitLimit;
                limiterOptions.QueueLimit = limits.ImageUpload.QueueLimit;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

        options.AddIpFixedWindowPolicy(RateLimitingPolicyNameConstants.SubmitContactUsForm, limits.SubmitContactUsForm);
        options.AddIpFixedWindowPolicy(RateLimitingPolicyNameConstants.InitiateDonation, limits.InitiateDonation);
        options.AddIpFixedWindowPolicy(RateLimitingPolicyNameConstants.AdminLogin, limits.AdminLogin);
    }

    private static void AddIpFixedWindowPolicy(
        this RateLimiterOptions options,
        string policyName,
        FixedWindowPolicyOptions policy)
    {
        options.AddPolicy(policyName, httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: GetClientPartitionKey(httpContext),
                factory: _ => CreateFixedWindowOptions(policy)));
    }

    private static FixedWindowRateLimiterOptions CreateFixedWindowOptions(FixedWindowPolicyOptions policy)
    {
        return new FixedWindowRateLimiterOptions
        {
            PermitLimit = policy.PermitLimit,
            Window = policy.Window,
            QueueLimit = 0,
            AutoReplenishment = true,
        };
    }
}

public static class RateLimitingPolicyNameConstants
{
    public const string SubmitContactUsForm = "submit-contact-us-form-rate-limiting-policy";
    public const string InitiateDonation = "initiate-donation-rate-limiting-policy";
    public const string AdminLogin = "admin-login-rate-limiting-policy";
    internal const string ImageUpload = "image-upload-rate-limiting-policy";
}
