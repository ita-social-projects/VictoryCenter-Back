using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class BaseHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageAnotherQuoteSectionLocalizationValidator()
    {
        RuleFor(x => x.QuoteText)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto.QuoteText),
                HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.QuoteTextMinLength,
                HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.QuoteTextMaxLength);

        RuleFor(x => x.AuthorName!)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto.AuthorName),
                HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.AuthorNameMinLength,
                HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.AuthorNameMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.AuthorName));
    }
}
