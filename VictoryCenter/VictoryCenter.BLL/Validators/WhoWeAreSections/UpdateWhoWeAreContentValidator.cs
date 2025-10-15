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

        RuleForEach(x => x.Contents)
            .NotNull().WithMessage(WhoWeAreConstants.ContentCanNotBeNull);

        RuleForEach(x => x.Contents)
            .SetValidator(content => new WhoWeAreSectionValidator(content.SectionType));
    }
}
