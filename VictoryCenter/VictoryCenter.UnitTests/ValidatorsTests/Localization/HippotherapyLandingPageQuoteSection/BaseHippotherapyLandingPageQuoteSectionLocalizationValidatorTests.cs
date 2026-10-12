using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageQuoteSection;

public class BaseHippotherapyLandingPageQuoteSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageQuoteSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageQuoteSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageQuoteSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenQuoteText_IsNullOrEmpty(string? quoteText)
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = quoteText!,
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuoteText_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMaxLength + 1),
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldNotHaveError_WhenAuthorName_IsNullOrEmpty(string? authorName)
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = authorName,
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorName_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuoteText_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMinLength - 1);
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = $"<p>{visibleText}</p>",
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuoteText_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "<p><br></p>",
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuoteText_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMinLength - 1),
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenQuoteText_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMaxLength);
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = $"<p><strong>{visibleText}</strong></p>",
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.QuoteText);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuoteText_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.QuoteTextMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = $"<p>{visibleText}</p>",
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorName_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMinLength - 1);
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorName_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = "<p><br></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorName_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenAuthorName_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMaxLength);
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorName_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageQuoteSectionLocalizationConstants.AuthorNameMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Valid quote text",
            AuthorName = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenQuoteText_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = string.Empty,
            AuthorName = "Valid author name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuoteText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageQuoteSectionLocalizationDto.QuoteText)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Hippotherapy changed my child's life.",
            AuthorName = "Parent of a patient",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValidWithoutAuthorName()
    {
        var model = new UpdateHippotherapyLandingPageQuoteSectionLocalizationDto
        {
            QuoteText = "Hippotherapy changed my child's life.",
            AuthorName = null,
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
