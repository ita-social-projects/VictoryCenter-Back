using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.Update;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnersSectionCommandValidator : AbstractValidator<UpdatePartnersSectionCommand>
{
    public UpdatePartnersSectionCommandValidator()
    {
        RuleFor(x => x.UpdateDto)
            .SetValidator(new UpdatePartnerSectionValidator());
    }
}
