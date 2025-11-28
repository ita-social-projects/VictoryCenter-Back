using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Update;

namespace VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

public class UpdateCorrespondentBankDetailsCommandValidator : AbstractValidator<UpdateCorrespondentBankDetailsCommand>
{
    public UpdateCorrespondentBankDetailsCommandValidator()
    {
        RuleFor(c => c.UpdateCorrespondentBankDetailsDto)
            .SetValidator(new BaseCorrespondentBankDetailsDtoValidator());
    }
}
