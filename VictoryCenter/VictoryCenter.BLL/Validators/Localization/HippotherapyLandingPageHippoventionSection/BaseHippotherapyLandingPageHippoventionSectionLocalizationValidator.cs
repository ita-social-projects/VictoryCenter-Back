using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionSection;

public class BaseHippotherapyLandingPageHippoventionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageHippoventionSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMaxLength);
    }
}
