using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

public class BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto.Title),
                HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMaxLength);
    }
}
