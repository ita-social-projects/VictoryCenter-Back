using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;

namespace VictoryCenter.BLL.Validators.Payment;

public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(PaymentRequestDto.Amount)));

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(PaymentRequestDto.Currency)));

        RuleFor(x => x.PaymentSystem)
            .IsInEnum().WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(PaymentRequestDto.PaymentSystem)));
    }
}
