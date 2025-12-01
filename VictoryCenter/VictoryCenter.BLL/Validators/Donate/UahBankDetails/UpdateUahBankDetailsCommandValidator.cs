using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;

namespace VictoryCenter.BLL.Validators.Donate.UahBankDetails;

public class UpdateUahBankDetailsCommandValidator : AbstractValidator<UpdateUahBankDetailsCommand>
{
    public UpdateUahBankDetailsCommandValidator()
    {
        RuleFor(command => command.UpdateUahBankDetailsDto)
            .SetValidator(new BaseUahBankDetailsDtoValidator());
    }
}
