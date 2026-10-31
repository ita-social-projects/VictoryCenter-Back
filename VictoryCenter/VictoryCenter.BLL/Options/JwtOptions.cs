using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options;

public class JwtOptions
{
    static readonly public string Position = "JwtOptions";

    [Required]
    public string Audience { get; init; } = null!;

    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    [Range(1, 60)]
    public int LifetimeInMinutes { get; init; }

    [Required]
    public string SecretKey { get; init; } = null!;

    [Required]
    public string RefreshTokenSecretKey { get; init; } = null!;

    [Required]
    [Range(1, 30)]
    public int RefreshTokenLifetimeInDays { get; init; }
}
