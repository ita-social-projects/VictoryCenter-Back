using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.WhoWeAreContents;

public class CreateWhoWeAreContentLocalizationValidator : AbstractValidator<CreateWhoWeAreContentLocalizationCommand>
{
    public CreateWhoWeAreContentLocalizationValidator()
    {
        RuleFor(x => x.ContentLocalizationDtos)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)));

        RuleForEach(x => x.ContentLocalizationDtos)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(CreateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)));

        RuleForEach(x => x.ContentLocalizationDtos)
            .SetValidator(new LocalizationIdentityValidator<CreateWhoWeAreContentLocalizationDto>());

        RuleForEach(x => x.ContentLocalizationDtos)
            .SetValidator(command => new WhoWeAreSectionLocalizationValidator(command.SectionType));
    }
}
