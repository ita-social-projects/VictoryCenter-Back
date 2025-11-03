using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.UahBankDetails;

public class UpdateUahBankDetailsCommandValidator : AbstractValidator<UpdateUahBankDetailsCommand>
{
    public UpdateUahBankDetailsCommandValidator()
    {
        RuleFor(command => command.UpdateUahBankDetailsDto)
            .SetValidator(new UahBankDetailsDtoValidator<UpdateUahBankDetailsDto>());
    }
}
