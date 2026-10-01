using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageIntroSection;

public class BaseHippotherapyLandingPageIntroSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageIntroSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageIntroSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Title),
                HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Description),
                HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMaxLength);
    }
}
