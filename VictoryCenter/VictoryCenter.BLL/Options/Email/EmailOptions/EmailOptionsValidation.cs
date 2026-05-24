using Microsoft.Extensions.Options;

namespace VictoryCenter.BLL.Options.Email.EmailOptions;

public class EmailOptionsValidation : IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string? name, EmailOptions options)
    {
        if (options.EmailProvider == EmailProvider.Resend
            && string.IsNullOrWhiteSpace(options.ResendApiToken))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(EmailOptions.ResendApiToken)} is required when " +
                $"{nameof(EmailOptions.EmailProvider)} is {EmailProvider.Resend}.");
        }

        return ValidateOptionsResult.Success;
    }
}
