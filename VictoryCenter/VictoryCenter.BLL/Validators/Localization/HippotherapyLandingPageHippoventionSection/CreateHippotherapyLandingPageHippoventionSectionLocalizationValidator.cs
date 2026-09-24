using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionSection;

public class CreateHippotherapyLandingPageHippoventionSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageHippoventionSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageHippoventionSectionLocalizationValidator(
        BaseHippotherapyLandingPageHippoventionSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageHippoventionSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageHippoventionSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageHippoventionSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
