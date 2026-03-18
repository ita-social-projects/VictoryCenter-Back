using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Localization.WhoWeAreContents;

public class UpdateWhoWeAreContentLocalizationValidator : AbstractValidator<UpdateWhoWeAreContentLocalizationCommand>
{
    public UpdateWhoWeAreContentLocalizationValidator()
    {
        RuleFor(x => x.ContentLocalizationDtos)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)));

        RuleForEach(x => x.ContentLocalizationDtos)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)));

        RuleForEach(x => x.ContentLocalizationDtos)
            .SetValidator(command => new WhoWeAreSectionLocalizationValidator(command.SectionType));
    }
}
