using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.Validators.Localization.EventsIntroSection;

public class BaseEventsIntroSectionLocalizationValidator
    : AbstractValidator<UpdateEventsIntroSectionLocalizationDto>
{
    public BaseEventsIntroSectionLocalizationValidator()
    {
        RuleFor(x => x.EventsBlockTitle)
            .MustHaveVisibleLength(
                nameof(UpdateEventsIntroSectionLocalizationDto.EventsBlockTitle),
                EventsIntroSectionLocalizationConstants.EventsBlockTitleMinLength,
                EventsIntroSectionLocalizationConstants.EventsBlockTitleMaxLength);

        RuleFor(x => x.PageDescription)
            .MustHaveVisibleLength(
                nameof(UpdateEventsIntroSectionLocalizationDto.PageDescription),
                EventsIntroSectionLocalizationConstants.PageDescriptionMinLength,
                EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength);
    }
}
