using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator(
        BaseHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
