using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.EventsIntroSection;

public class CreateEventsIntroSectionLocalizationValidator
    : AbstractValidator<CreateEventsIntroSectionLocalizationCommand>
{
    public CreateEventsIntroSectionLocalizationValidator(
        BaseEventsIntroSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateEventsIntroSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateEventsIntroSectionLocalizationDto>());
        RuleFor(c => c.CreateEventsIntroSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
