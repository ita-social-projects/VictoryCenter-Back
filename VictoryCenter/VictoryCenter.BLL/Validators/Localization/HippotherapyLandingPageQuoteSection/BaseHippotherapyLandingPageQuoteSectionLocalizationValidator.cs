using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.Helpers;

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

        RuleFor(x => x.AuthorName)
            .Cascade(CascadeMode.Stop)
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMinLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                    nameof(UpdateHippotherapyLandingPageQuoteSectionLocalizationDto.AuthorName),
                    HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMaxLength)
                .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                    nameof(UpdateHippotherapyLandingPageQuoteSectionLocalizationDto.AuthorName),
                    HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.AuthorName));
    }
}
