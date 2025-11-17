using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;

public class ForeignBankDetailsDtoValidator<T> : AbstractValidator<T>
    where T : IForeignBankDetails
{
    public ForeignBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)));

        RuleFor(dto => dto.Iban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Iban)));
    }
}
