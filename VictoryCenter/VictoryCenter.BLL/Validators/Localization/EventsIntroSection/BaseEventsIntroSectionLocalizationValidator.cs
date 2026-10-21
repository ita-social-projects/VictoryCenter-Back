using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.Validators.Localization.EventsIntroSection;

public class BaseEventsIntroSectionLocalizationValidator
    : AbstractValidator<UpdateEventsIntroSectionLocalizationDto>
{
    public BaseEventsIntroSectionLocalizationValidator()
    {
        RuleFor(x => x)
            .Must(x => x.EventsBlockTitle is not null || x.PageDescription is not null)
            .WithMessage("At least one localization field must be provided.");

        RuleFor(x => x.EventsBlockTitle!)
            .MustHaveVisibleLength(
                nameof(UpdateEventsIntroSectionLocalizationDto.EventsBlockTitle),
                EventsIntroSectionLocalizationConstants.EventsBlockTitleMinLength,
                EventsIntroSectionLocalizationConstants.EventsBlockTitleMaxLength)
            .When(x => x.EventsBlockTitle is not null);

        RuleFor(x => x.PageDescription!)
            .MustHaveVisibleLength(
                nameof(UpdateEventsIntroSectionLocalizationDto.PageDescription),
                EventsIntroSectionLocalizationConstants.PageDescriptionMinLength,
                EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength)
            .When(x => x.PageDescription is not null);
    }
}
