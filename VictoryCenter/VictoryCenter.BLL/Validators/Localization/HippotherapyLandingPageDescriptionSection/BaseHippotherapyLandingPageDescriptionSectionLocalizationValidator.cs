using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;

public class BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength);
    }
}
