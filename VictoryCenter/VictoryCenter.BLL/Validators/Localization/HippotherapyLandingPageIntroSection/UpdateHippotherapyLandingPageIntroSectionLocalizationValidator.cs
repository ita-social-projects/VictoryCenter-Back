using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageIntroSection;

public class UpdateHippotherapyLandingPageIntroSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageIntroSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageIntroSectionLocalizationValidator(
        BaseHippotherapyLandingPageIntroSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageIntroSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
