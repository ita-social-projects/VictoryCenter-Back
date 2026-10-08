using FluentValidation;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageQuoteSection;

public class BaseHippotherapyLandingPageQuoteSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageQuoteSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageQuoteSectionLocalizationValidator()
    {
        RuleFor(x => x.QuoteText)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageQuoteSectionLocalizationDto.QuoteText),
                HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMinLength,
                HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMaxLength);

        RuleFor(x => x.AuthorName!)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageQuoteSectionLocalizationDto.AuthorName),
                HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMinLength,
                HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.AuthorName));
    }
}
