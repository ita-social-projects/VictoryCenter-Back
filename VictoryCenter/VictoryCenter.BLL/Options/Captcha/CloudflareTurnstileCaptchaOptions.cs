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
}
