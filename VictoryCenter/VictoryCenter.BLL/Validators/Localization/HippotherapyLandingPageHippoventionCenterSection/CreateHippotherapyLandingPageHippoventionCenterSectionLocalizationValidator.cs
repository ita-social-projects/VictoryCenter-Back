using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator(
        BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
