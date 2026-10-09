using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

public class CreateHippotherapyLandingPageAdvantagesSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
        BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
