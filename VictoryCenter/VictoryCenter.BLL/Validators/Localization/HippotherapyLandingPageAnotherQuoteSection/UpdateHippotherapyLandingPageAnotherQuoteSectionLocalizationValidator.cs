using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator(
        BaseHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
