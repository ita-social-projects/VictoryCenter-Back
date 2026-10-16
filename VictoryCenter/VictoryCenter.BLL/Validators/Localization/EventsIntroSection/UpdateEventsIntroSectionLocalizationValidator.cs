using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.EventsIntroSection;

public class UpdateEventsIntroSectionLocalizationValidator
    : AbstractValidator<UpdateEventsIntroSectionLocalizationCommand>
{
    public UpdateEventsIntroSectionLocalizationValidator(
        BaseEventsIntroSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateEventsIntroSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
