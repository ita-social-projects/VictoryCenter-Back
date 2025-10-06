using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.WhoWeAreSections;

public class UpdateWhoWeAreContentValidator : AbstractValidator<UpdateWhoWeAreContentCommand>
{
    public UpdateWhoWeAreContentValidator()
    {
        RuleFor(x => x.SectionType)
            .IsInEnum();

        RuleForEach(x => x.Content)
            .NotNull().WithMessage(WhoWeAreConstants.ContentCanNotBeNull)
            .SetValidator(content => new WhoWeAreSectionValidator(content.SectionType));
    }
}
