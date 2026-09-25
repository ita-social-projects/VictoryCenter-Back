using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionSection;

public class UpdateHippotherapyLandingPageHippoventionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageHippoventionSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageHippoventionSectionLocalizationValidator(
        BaseHippotherapyLandingPageHippoventionSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
