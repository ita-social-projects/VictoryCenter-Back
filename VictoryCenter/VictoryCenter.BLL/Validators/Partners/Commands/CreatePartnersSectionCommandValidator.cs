using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.BLL.Validators.Partners.Commands;

public class CreatePartnersSectionCommandValidator : AbstractValidator<CreatePartnersSectionCommand>
{
    public CreatePartnersSectionCommandValidator()
    {
        RuleFor(x => x.CreatePartnersSectionDto)
            .NotNull()
            .SetValidator(new CreatePartnerSectionValidator());
    }
}
