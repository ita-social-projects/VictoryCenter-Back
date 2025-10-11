using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.Donate.SupportOptions;
public class SupportOptionsDtoValidator<T> : AbstractValidator<T>
    where T : CreateSupportOptionsDto
{
    public SupportOptionsDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));

        RuleFor(dto => dto.Value)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));

        RuleFor(dto => dto.Currency)
            .Must(currency => currency is BankCurrency.Usd or BankCurrency.Eur or BankCurrency.Uah)
            .WithMessage(SupportOptionsConstants.OnlyUsdOrEurOrUahMassage);
    }
}
