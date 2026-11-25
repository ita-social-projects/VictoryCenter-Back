using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options.Captcha;

public class CloudflareTurnstileCaptchaOptions
{
    public static readonly string Position = "CloudflareTurnstileCaptchaOptions";

    [Required]
    public required string SecretKey { get; init; }

    [Required]
    [Url]
    public required string SiteVerifyUrl { get; init; }

    [Range(100, 30000)]
    public int VerificationTimeoutMilliseconds { get; init; } = 10000;

    [Range(1, 3)]
    public int MaxAttempts { get; init; } = 3;

    [Range(0, 1000)]
    public int RetryDelayMilliseconds { get; init; } = 200;
}
