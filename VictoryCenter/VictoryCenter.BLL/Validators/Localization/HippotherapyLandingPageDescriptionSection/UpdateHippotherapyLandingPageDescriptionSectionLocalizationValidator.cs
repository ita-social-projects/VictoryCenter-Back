using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;

public class UpdateHippotherapyLandingPageDescriptionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageDescriptionSectionLocalizationValidator(
        BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
