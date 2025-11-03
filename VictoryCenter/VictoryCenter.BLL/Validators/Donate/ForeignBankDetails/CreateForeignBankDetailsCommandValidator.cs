using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;

public class CreateForeignBankDetailsCommandValidator : AbstractValidator<CreateForeignBankDetailsCommand>
{
    public CreateForeignBankDetailsCommandValidator()
    {
        RuleFor(command => command.CreateForeignBankDetailsDto)
            .SetValidator(new ForeignBankDetailsDtoValidator<CreateForeignBankDetailsDto>());

        RuleFor(command => command.CreateForeignBankDetailsDto.Currency)
            .Must(currency => currency is BankCurrency.Usd or BankCurrency.Eur)
            .WithMessage(ForeignBankDetailsConstants.OnlyUsdOrEurMessage);
    }
}
