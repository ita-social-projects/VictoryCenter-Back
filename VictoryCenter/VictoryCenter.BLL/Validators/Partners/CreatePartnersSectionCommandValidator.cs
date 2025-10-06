using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;

namespace VictoryCenter.BLL.Validators.Partners;

public class CreatePartnersSectionCommandValidator : AbstractValidator<CreatePartnersSectionCommand>
{
    public CreatePartnersSectionCommandValidator()
    {
        RuleFor(x => x.CreatePartnersSectionDto)
            .SetValidator(new CreatePartnerSectionValidator());
    }
}
