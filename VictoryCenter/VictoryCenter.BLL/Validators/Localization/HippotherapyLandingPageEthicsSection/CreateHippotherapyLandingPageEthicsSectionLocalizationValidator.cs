using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;

public class CreateHippotherapyLandingPageEthicsSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageEthicsSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageEthicsSectionLocalizationValidator(
        BaseHippotherapyLandingPageEthicsSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageEthicsSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageEthicsSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageEthicsSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
