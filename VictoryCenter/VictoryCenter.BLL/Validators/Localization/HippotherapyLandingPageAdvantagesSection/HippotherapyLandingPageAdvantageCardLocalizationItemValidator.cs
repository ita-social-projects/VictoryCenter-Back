using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

public class HippotherapyLandingPageAdvantageCardLocalizationItemValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto>
{
    public HippotherapyLandingPageAdvantageCardLocalizationItemValidator()
    {
        RuleFor(x => x.CardId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto.CardId)));

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto.Description),
                HippotherapyLandingPageAdvantagesSectionLocalizationConstants.CardDescriptionMinLength,
                HippotherapyLandingPageAdvantagesSectionLocalizationConstants.CardDescriptionMaxLength);
    }
}
