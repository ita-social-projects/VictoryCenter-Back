using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;
public class UpdateForeignBankDetailsCommandValidator : AbstractValidator<UpdateForeignBankDetailsCommand>
{
    public UpdateForeignBankDetailsCommandValidator()
    {
        RuleFor(command => command.UpdateForeignBankDetailsDto)
            .SetValidator(new ForeignBankDetailsDtoValidator<UpdateForeignBankDetailsDto>());
    }
}
