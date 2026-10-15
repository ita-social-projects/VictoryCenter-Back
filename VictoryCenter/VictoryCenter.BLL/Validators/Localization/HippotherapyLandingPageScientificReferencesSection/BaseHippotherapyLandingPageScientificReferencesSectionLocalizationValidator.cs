using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

public class BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.Title),
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.Description),
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMaxLength);
    }
}
