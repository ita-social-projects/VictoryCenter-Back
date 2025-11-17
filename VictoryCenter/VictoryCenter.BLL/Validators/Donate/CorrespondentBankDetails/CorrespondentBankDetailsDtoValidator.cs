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
    }
}
