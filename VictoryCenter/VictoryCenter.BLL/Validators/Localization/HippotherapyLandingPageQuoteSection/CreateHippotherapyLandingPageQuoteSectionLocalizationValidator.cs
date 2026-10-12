using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageQuoteSection;

public class CreateHippotherapyLandingPageQuoteSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageQuoteSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageQuoteSectionLocalizationValidator(
        BaseHippotherapyLandingPageQuoteSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageQuoteSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageQuoteSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageQuoteSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
