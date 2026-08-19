using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateQuoteSectionDtoValidatorTests
{
    private readonly UpdateQuoteSectionDtoValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_QuoteTextIsNullOrEmpty_ShouldHaveError(string? quoteText)
    {
        // Arrange
        var dto = GetValidDto() with { QuoteText = quoteText! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuoteText)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateQuoteSectionDto.QuoteText)));
    }

    [Fact]
    public void Validate_QuoteTextIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.TextMinLength - 1);
        var dto = GetValidDto() with { QuoteText = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuoteText)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateQuoteSectionDto.QuoteText), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_QuoteTextIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.QuoteTextMaxLength + 1);
        var dto = GetValidDto() with { QuoteText = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuoteText)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateQuoteSectionDto.QuoteText), HippotherapyLandingPageConstants.QuoteTextMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_AuthorNameIsNullOrWhitespace_ShouldNotHaveError(string? authorName)
    {
        // Arrange
        var dto = GetValidDto() with { AuthorName = authorName };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AuthorName);
    }

    [Fact]
    public void Validate_AuthorNameIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.TextMinLength - 1);
        var dto = GetValidDto() with { AuthorName = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorName)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateQuoteSectionDto.AuthorName), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_AuthorNameIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.QuoteAuthorNameMaxLength + 1);
        var dto = GetValidDto() with { AuthorName = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorName)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateQuoteSectionDto.AuthorName), HippotherapyLandingPageConstants.QuoteAuthorNameMaxLength));
    }

    [Fact]
    public void Validate_AuthorNameIsValid_ShouldNotHaveError()
    {
        // Arrange
        var dto = GetValidDto() with { AuthorName = new string('A', HippotherapyLandingPageConstants.TextMinLength) };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AuthorName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ImageIdIsNotPositive_ShouldHaveError(long imageId)
    {
        // Arrange
        var dto = GetValidDto() with { ImageId = imageId };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ImageId)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateQuoteSectionDto.ImageId)));
    }

    [Fact]
    public void Validate_ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = GetValidDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateQuoteSectionDto GetValidDto() => new()
    {
        QuoteText = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        AuthorName = null,
        ImageId = 1,
    };
}
