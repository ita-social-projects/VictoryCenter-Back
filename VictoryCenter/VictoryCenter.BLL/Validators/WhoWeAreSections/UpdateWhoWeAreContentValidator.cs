using FluentValidation;
using VictoryCenter.BLL.Commands.WhoWeAre.Update;

namespace VictoryCenter.BLL.Validators.WhoWeAreSections;

public class UpdateWhoWeAreContentValidator : AbstractValidator<UpdateWhoWeAreContentCommand>
{
    public UpdateWhoWeAreContentValidator()
    {
        RuleFor(x => x.SectionType)
            .IsInEnum();

        RuleForEach(x => x.Content)
            .SetValidator(content => new WhoWeAreSectionValidator(content.SectionType));
    }
}
