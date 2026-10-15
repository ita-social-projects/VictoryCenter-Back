using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

public class UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationValidator(
        BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
