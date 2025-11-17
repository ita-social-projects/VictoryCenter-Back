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

        RuleFor(x => x.Contents)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateWhoWeAreContentCommand.Contents)));

        RuleForEach(x => x.Contents)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateWhoWeAreContentCommand.Contents)));

        RuleForEach(x => x.Contents)
            .SetValidator(command => new WhoWeAreSectionValidator(command.SectionType));
    }
}
