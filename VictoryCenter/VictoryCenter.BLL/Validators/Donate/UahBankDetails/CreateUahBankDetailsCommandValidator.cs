using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;

namespace VictoryCenter.BLL.Validators.Donate.UahBankDetails;

public class CreateUahBankDetailsCommandValidator : AbstractValidator<CreateUahBankDetailsCommand>
{
    public CreateUahBankDetailsCommandValidator()
    {
        RuleFor(command => command.CreateUahBankDetailsDto)
            .SetValidator(new BaseUahBankDetailsDtoValidator());
    }
}
