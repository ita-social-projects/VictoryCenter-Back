using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

public class CreateHippotherapyLandingPageScientificReferencesSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageScientificReferencesSectionLocalizationValidator(
        BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
