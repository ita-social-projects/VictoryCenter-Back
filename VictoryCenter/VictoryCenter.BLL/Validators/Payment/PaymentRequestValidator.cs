using FluentValidation;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.BLL.Validators.Payment;

public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
{
    public PaymentRequestValidator(IOptions<WayForPayOptions> wayForPayOptions)
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(PaymentRequestDto.Amount)));

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(PaymentRequestDto.Currency)));

        RuleFor(x => x.PaymentSystem)
            .IsInEnum().WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(PaymentRequestDto.PaymentSystem)));

        RuleFor(x => x.ReturnUrl)
            .Must(returnUrl => IsTrustedReturnUrl(returnUrl, wayForPayOptions.Value.AllowedReturnUrlHosts))
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(PaymentRequestDto.ReturnUrl)));
    }

    private static bool IsTrustedReturnUrl(string? returnUrl, IEnumerable<string> allowedHosts)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return true;
        }

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var usesSecureDefaultPort = uri.Scheme == Uri.UriSchemeHttps && uri.IsDefaultPort;
        var usesLocalDevelopmentUrl = uri.IsLoopback
                                      && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        return (usesSecureDefaultPort || usesLocalDevelopmentUrl)
               && allowedHosts.Any(allowedHost => string.Equals(
                   uri.IdnHost.TrimEnd('.'),
                   allowedHost.Trim().TrimEnd('.'),
                   StringComparison.OrdinalIgnoreCase));
    }
}
