using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;

namespace VictoryCenter.BLL.Validators.WhoWeAreSections;

public class UpdateWhoWeAreContentValidator : AbstractValidator<UpdateWhoWeAreContentCommand>
{
    public UpdateWhoWeAreContentValidator()
    {
        RuleFor(x => x.SectionType)
            .IsInEnum();

        RuleForEach(x => x.Content)
            .NotNull().WithMessage("Content cannot be null.")
            .SetValidator(content => new WhoWeAreSectionValidator(content.SectionType));
    }
}
