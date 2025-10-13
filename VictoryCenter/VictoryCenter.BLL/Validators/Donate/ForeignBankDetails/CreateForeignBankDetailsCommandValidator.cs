using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;
public class CreateForeignBankDetailsCommandValidator : AbstractValidator<CreateForeignBankDetailsCommand>
{
    public CreateForeignBankDetailsCommandValidator()
    {
        RuleFor(command => command.CreateForeignBankDetailsDto)
            .SetValidator(new ForeignBankDetailsDtoValidator<CreateForeignBankDetailsDto>());
    }
}
