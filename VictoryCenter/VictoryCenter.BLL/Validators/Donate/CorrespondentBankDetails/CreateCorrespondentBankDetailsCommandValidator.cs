using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Create;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

public class CreateCorrespondentBankDetailsCommandValidator : AbstractValidator<CreateCorrespondentBankDetailsCommand>
{
    public CreateCorrespondentBankDetailsCommandValidator()
    {
        RuleFor(c => c.CreateCorrespondentBankDetailsDto)
            .SetValidator(new CorrespondentBankDetailsDtoValidator<CreateCorrespondentBankDetailsDto>());
    }
}
