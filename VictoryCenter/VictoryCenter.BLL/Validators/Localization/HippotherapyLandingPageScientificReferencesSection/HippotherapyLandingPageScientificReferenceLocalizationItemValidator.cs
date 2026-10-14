using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

public class HippotherapyLandingPageScientificReferenceLocalizationItemValidator
    : AbstractValidator<UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto>
{
    public HippotherapyLandingPageScientificReferenceLocalizationItemValidator()
    {
        RuleFor(x => x.ReferenceId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto.ReferenceId)));

        RuleFor(x => x.Name)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto.Name),
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.ReferenceNameMinLength,
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.ReferenceNameMaxLength);
    }
}
