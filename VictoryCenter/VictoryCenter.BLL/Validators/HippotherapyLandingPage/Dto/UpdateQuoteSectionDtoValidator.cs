using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

public class UpdateQuoteSectionDtoValidator : AbstractValidator<UpdateQuoteSectionDto>
{
    public UpdateQuoteSectionDtoValidator()
    {
        RuleFor(x => x.QuoteText)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateQuoteSectionDto.QuoteText)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateQuoteSectionDto.QuoteText), HippotherapyLandingPageConstants.TextMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.QuoteTextMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateQuoteSectionDto.QuoteText), HippotherapyLandingPageConstants.QuoteTextMaxLength));

        RuleFor(x => x.AuthorName)
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateQuoteSectionDto.AuthorName), HippotherapyLandingPageConstants.TextMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageConstants.QuoteAuthorNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateQuoteSectionDto.AuthorName), HippotherapyLandingPageConstants.QuoteAuthorNameMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.AuthorName));

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateQuoteSectionDto.ImageId)))
            .When(x => x.ImageId.HasValue);
    }
}
