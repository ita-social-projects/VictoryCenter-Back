using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

public class UpdateHippotherapyLandingPageAdvantagesSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
        BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
