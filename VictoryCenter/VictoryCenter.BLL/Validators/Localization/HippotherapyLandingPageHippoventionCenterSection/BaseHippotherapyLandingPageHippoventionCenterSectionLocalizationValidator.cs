using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMaxLength);

        RuleFor(x => x.Pros)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Pros),
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMinLength,
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMaxLength);
    }
}
