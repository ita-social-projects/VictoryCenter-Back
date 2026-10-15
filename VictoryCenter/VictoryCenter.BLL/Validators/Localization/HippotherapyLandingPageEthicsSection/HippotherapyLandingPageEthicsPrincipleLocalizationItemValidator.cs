using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;

public class HippotherapyLandingPageEthicsPrincipleLocalizationItemValidator
    : AbstractValidator<UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto>
{
    public HippotherapyLandingPageEthicsPrincipleLocalizationItemValidator()
    {
        RuleFor(x => x.PrincipleId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto.PrincipleId)));

        RuleFor(x => x.Text)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto.Text),
                HippotherapyLandingPageEthicsSectionLocalizationConstants.PrincipleTextMinLength,
                HippotherapyLandingPageEthicsSectionLocalizationConstants.PrincipleTextMaxLength);
    }
}
