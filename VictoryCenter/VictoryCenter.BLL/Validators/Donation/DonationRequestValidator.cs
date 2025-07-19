using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Donation;

namespace VictoryCenter.BLL.Validators.Donation;

public class DonationRequestValidator : AbstractValidator<DonationRequestDto>
{
    private const int CurrencyCodeLenght = 3;
    private const string InvalidCurrencyCode = "Invalid currency code";
    private const string CurrencyExpression = "^[A-Z]{3}$";

    public DonationRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(DonationRequestDto.Amount), 0));

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(DonationRequestDto.Currency)))
            .Matches(CurrencyExpression).WithMessage(InvalidCurrencyCode)
            .Length(CurrencyCodeLenght).WithMessage(ErrorMessagesConstants.PropertyMustHaveALengthOfNCharacters(nameof(DonationRequestDto.Currency), CurrencyCodeLenght));

        RuleFor(x => x.PaymentSystem)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(DonationRequestDto.PaymentSystem)));
    }
}
