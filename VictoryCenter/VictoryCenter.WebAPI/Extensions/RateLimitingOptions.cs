using Microsoft.Extensions.Options;

namespace VictoryCenter.WebAPI.Extensions;

/// <summary>
/// Limits for the rate-limiting policies, bound from the <see cref="SectionName"/> configuration section.
/// </summary>
/// <remarks>
/// Every property has a default, so the section is optional. An environment overrides only the values it needs.
/// </remarks>
public sealed class RateLimitingOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "RateLimiting";

    /// <summary>
    /// Gets or sets the limit for Swagger UI and documents outside Development.
    /// </summary>
    public FixedWindowPolicyOptions Swagger { get; set; } = new() { PermitLimit = 60, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the default limit for anonymous public read endpoints.
    /// </summary>
    public FixedWindowPolicyOptions PublicRead { get; set; } = new() { PermitLimit = 60, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the limit for anonymous endpoints that stream a file from storage.
    /// </summary>
    public FixedWindowPolicyOptions FileDownload { get; set; } = new() { PermitLimit = 20, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the limit for anonymous SignalR hub requests (negotiate, connect, and long-polling polls).
    /// </summary>
    public FixedWindowPolicyOptions RealtimeConnect { get; set; } = new() { PermitLimit = 30, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the limit for the admin login endpoint.
    /// </summary>
    public FixedWindowPolicyOptions AdminLogin { get; set; } = new() { PermitLimit = 5, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the limit for the anonymous refresh-token endpoint.
    /// </summary>
    public FixedWindowPolicyOptions AuthRefresh { get; set; } = new() { PermitLimit = 30, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the limit for starting a donation through the payment provider.
    /// </summary>
    public FixedWindowPolicyOptions InitiateDonation { get; set; } = new() { PermitLimit = 10, WindowSeconds = 60 };

    /// <summary>
    /// Gets or sets the limit for submitting the contact-us form, which sends an email.
    /// </summary>
    public FixedWindowPolicyOptions SubmitContactUsForm { get; set; } = new() { PermitLimit = 10, WindowSeconds = 24 * 60 * 60 };

    /// <summary>
    /// Gets or sets the concurrency limit for image uploads.
    /// </summary>
    public ConcurrencyPolicyOptions ImageUpload { get; set; } = new() { PermitLimit = 2, QueueLimit = 64 };

    internal IEnumerable<string> GetValidationErrors()
    {
        var fixedWindowPolicies = new Dictionary<string, FixedWindowPolicyOptions?>
        {
            [nameof(Swagger)] = Swagger,
            [nameof(PublicRead)] = PublicRead,
            [nameof(FileDownload)] = FileDownload,
            [nameof(RealtimeConnect)] = RealtimeConnect,
            [nameof(AdminLogin)] = AdminLogin,
            [nameof(AuthRefresh)] = AuthRefresh,
            [nameof(InitiateDonation)] = InitiateDonation,
            [nameof(SubmitContactUsForm)] = SubmitContactUsForm,
        };

        foreach (var (name, policy) in fixedWindowPolicies)
        {
            if (policy is null || policy.PermitLimit < 1 || policy.WindowSeconds < 1)
            {
                yield return $"{SectionName}:{name} must have PermitLimit >= 1 and WindowSeconds >= 1.";
            }
        }

        if (ImageUpload is null || ImageUpload.PermitLimit < 1 || ImageUpload.QueueLimit < 0)
        {
            yield return $"{SectionName}:{nameof(ImageUpload)} must have PermitLimit >= 1 and QueueLimit >= 0.";
        }
    }
}

/// <summary>
/// Fails startup when a rate-limiting policy has a non-positive limit or window.
/// </summary>
public sealed class RateLimitingOptionsValidator : IValidateOptions<RateLimitingOptions>
{
    public ValidateOptionsResult Validate(string? name, RateLimitingOptions options)
    {
        var errors = options.GetValidationErrors().ToList();

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }
}

/// <summary>
/// Settings for a fixed-window policy partitioned by client IP.
/// </summary>
public sealed class FixedWindowPolicyOptions
{
    /// <summary>
    /// Gets or sets the number of requests a client may make per window.
    /// </summary>
    public int PermitLimit { get; set; }

    /// <summary>
    /// Gets or sets the window length in seconds.
    /// </summary>
    public int WindowSeconds { get; set; }

    /// <summary>
    /// Gets the window length.
    /// </summary>
    public TimeSpan Window => TimeSpan.FromSeconds(WindowSeconds);
}

/// <summary>
/// Settings for a concurrency policy.
/// </summary>
public sealed class ConcurrencyPolicyOptions
{
    /// <summary>
    /// Gets or sets the number of requests processed at the same time.
    /// </summary>
    public int PermitLimit { get; set; }

    /// <summary>
    /// Gets or sets the number of requests that wait for a free permit before new ones are rejected.
    /// </summary>
    public int QueueLimit { get; set; }
}
