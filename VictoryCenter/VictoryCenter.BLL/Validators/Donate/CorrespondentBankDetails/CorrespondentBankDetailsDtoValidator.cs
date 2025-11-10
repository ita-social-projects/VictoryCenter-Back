using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

public class CorrespondentBankDetailsDtoValidator<T> : AbstractValidator<T>
    where T : CreateCorrespondentBankDetailsDto
{
    public CorrespondentBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Name)));

        RuleFor(dto => dto.Account)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Account)));
    }
}
