using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageIntroSection;

public class CreateHippotherapyLandingPageIntroSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageIntroSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageIntroSectionLocalizationValidator(
        BaseHippotherapyLandingPageIntroSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageIntroSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageIntroSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageIntroSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
