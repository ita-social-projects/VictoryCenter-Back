using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options;

public class JwtOptions
{
    public const string Position = "JwtOptions";

    [Required]
    public string Audience { get; init; } = null!;

    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public int LifetimeInMinutes { get; init; }

    [Required]
    public string SecretKey { get; init; } = null!;

    [Required]
    public string RefreshTokenSecretKey { get; init; } = null!;
}
