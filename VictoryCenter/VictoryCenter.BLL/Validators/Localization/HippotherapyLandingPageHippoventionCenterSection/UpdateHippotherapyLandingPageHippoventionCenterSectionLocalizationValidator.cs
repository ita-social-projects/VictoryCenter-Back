using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator(
        BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
