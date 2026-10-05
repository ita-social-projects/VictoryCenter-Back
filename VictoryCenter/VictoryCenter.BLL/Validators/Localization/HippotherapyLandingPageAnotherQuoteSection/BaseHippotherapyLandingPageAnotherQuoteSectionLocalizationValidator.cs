using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.Helpers;

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

        RuleFor(x => x.AuthorName)
            .Cascade(CascadeMode.Stop)
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.AuthorNameMinLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                    nameof(UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto.AuthorName),
                    HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.AuthorNameMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.AuthorNameMaxLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                    nameof(UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto.AuthorName),
                    HippotherapyLandingPageAnotherQuoteSectionLocalizationConstants.AuthorNameMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.AuthorName));
    }
}
