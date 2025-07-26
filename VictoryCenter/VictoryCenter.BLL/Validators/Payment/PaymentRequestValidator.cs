using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Common;

namespace VictoryCenter.BLL.Validators.Payment;

public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(PaymentRequestDto.Amount), 0));

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(PaymentRequestDto.Currency)));

        RuleFor(x => x.PaymentSystem)
            .IsInEnum().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(PaymentRequestDto.PaymentSystem)));
    }
}
