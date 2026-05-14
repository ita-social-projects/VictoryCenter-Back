using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VictoryCenter.BLL.Options.Email.EmailOptions;

public class EmailOptions : IValidatableObject
{
    public static readonly string Position = "EmailOptions";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EmailProvider EmailProvider { get; init; }

    public string? ResendApiToken { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EmailProvider == EmailProvider.Resend
            && string.IsNullOrWhiteSpace(ResendApiToken))
        {
            yield return new ValidationResult(
                $"{nameof(ResendApiToken)} is required when {nameof(EmailProvider)} is {EmailProvider.Resend}.",
                [nameof(ResendApiToken)]);
        }
    }
}

public enum EmailProvider
{
    Resend,
    Dummy
}
