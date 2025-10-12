using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Partners.Update;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.BLL.Validators.Partners.Commands;

public class UpdatePartnersSectionCommandValidator : AbstractValidator<UpdatePartnersSectionCommand>
{
    public UpdatePartnersSectionCommandValidator()
    {
        RuleFor(x => x.UpdateDto)
            .NotNull()
            .SetValidator(new UpdatePartnerSectionValidator());
    }
}
