using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VictoryCenter.BLL.Options;

public class JwtOptions : IValidatableObject
{
    public const string Position = "JwtOptions";
    private const int MinimumSigningKeyBytes = 32;

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Encoding.UTF8.GetByteCount(SecretKey ?? string.Empty) < MinimumSigningKeyBytes)
        {
            yield return new ValidationResult(
                $"{nameof(SecretKey)} must contain at least {MinimumSigningKeyBytes} UTF-8 bytes for HS256.",
                [nameof(SecretKey)]);
        }

        if (Encoding.UTF8.GetByteCount(RefreshTokenSecretKey ?? string.Empty) < MinimumSigningKeyBytes)
        {
            yield return new ValidationResult(
                $"{nameof(RefreshTokenSecretKey)} must contain at least {MinimumSigningKeyBytes} UTF-8 bytes for HS256.",
                [nameof(RefreshTokenSecretKey)]);
        }
    }
}
