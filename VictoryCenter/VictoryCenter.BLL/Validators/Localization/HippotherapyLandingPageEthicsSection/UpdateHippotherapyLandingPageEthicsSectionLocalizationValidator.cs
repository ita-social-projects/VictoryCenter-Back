using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;

public class UpdateHippotherapyLandingPageEthicsSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageEthicsSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageEthicsSectionLocalizationValidator(
        BaseHippotherapyLandingPageEthicsSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageEthicsSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
