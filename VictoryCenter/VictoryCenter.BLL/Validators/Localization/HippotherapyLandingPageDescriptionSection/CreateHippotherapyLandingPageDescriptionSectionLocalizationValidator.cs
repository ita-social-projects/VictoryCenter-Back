using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;

public class CreateHippotherapyLandingPageDescriptionSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageDescriptionSectionLocalizationValidator(
        BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageDescriptionSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageDescriptionSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageDescriptionSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
