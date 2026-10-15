using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;

public class BaseHippotherapyLandingPageEthicsSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageEthicsSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageEthicsSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Title),
                HippotherapyLandingPageEthicsSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageEthicsSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Description),
                HippotherapyLandingPageEthicsSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageEthicsSectionLocalizationConstants.DescriptionMaxLength);
    }
}
