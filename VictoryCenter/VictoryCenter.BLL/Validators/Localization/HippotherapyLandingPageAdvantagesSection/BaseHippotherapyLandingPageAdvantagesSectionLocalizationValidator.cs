using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

public class BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Title),
                HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMaxLength);
    }
}
