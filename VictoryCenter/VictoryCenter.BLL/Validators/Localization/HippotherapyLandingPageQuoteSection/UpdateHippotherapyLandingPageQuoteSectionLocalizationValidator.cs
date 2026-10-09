using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageQuoteSection;

public class UpdateHippotherapyLandingPageQuoteSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageQuoteSectionLocalizationValidator(
        BaseHippotherapyLandingPageQuoteSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageQuoteSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
