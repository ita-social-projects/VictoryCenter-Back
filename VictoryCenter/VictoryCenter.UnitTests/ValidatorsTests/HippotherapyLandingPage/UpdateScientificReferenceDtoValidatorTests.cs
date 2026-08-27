using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateScientificReferenceDtoValidatorTests
{
    private readonly UpdateScientificReferenceDtoValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_NameIsNullOrEmpty_ShouldHaveError(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferenceDto.Name)));
    }

    [Fact]
    public void Validate_NameIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.ScientificReferenceNameMinLength - 1);
        var dto = GetValidDto() with { Name = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                  nameof(UpdateScientificReferenceDto.Name), HippotherapyLandingPageConstants.ScientificReferenceNameMinLength));
    }

    [Fact]
    public void Validate_NameIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.ScientificReferenceNameMaxLength + 1);
        var dto = GetValidDto() with { Name = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                  nameof(UpdateScientificReferenceDto.Name), HippotherapyLandingPageConstants.ScientificReferenceNameMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_UrlIsNullOrWhitespace_ShouldHaveRequiredError(string? url)
    {
        // Arrange
        var dto = GetValidDto() with { Url = url! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferenceDto.Url)));
    }

    [Fact]
    public void Validate_UrlIsEmpty_ShouldNotAlsoHaveFormatError()
    {
        var dto = GetValidDto() with { Url = string.Empty };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            e => e.PropertyName == nameof(UpdateScientificReferenceDto.Url)
                 && e.ErrorMessage == ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(UpdateScientificReferenceDto.Url)));
    }

    [Fact]
    public void Validate_UrlIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('a', HippotherapyLandingPageConstants.ScientificReferenceUrlMinLength - 1);
        var dto = GetValidDto() with { Url = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                  nameof(UpdateScientificReferenceDto.Url), HippotherapyLandingPageConstants.ScientificReferenceUrlMinLength));
    }

    [Fact]
    public void Validate_UrlIsTooLong_ShouldHaveError()
    {
        // Arrange
        var padding = new string('a', HippotherapyLandingPageConstants.ScientificReferenceUrlMaxLength);
        var tooLong = $"https://example.com/{padding}";
        var dto = GetValidDto() with { Url = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                  nameof(UpdateScientificReferenceDto.Url), HippotherapyLandingPageConstants.ScientificReferenceUrlMaxLength));
    }

    [Fact]
    public void Validate_UrlIsNotAbsoluteUri_ShouldHaveFormatError()
    {
        // Arrange
        var dto = GetValidDto() with { Url = "not a valid url" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(UpdateScientificReferenceDto.Url)));
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

    private static UpdateScientificReferenceDto GetValidDto() => new()
    {
        Id = null,
        Name = new string('A', HippotherapyLandingPageConstants.ScientificReferenceNameMinLength),
        Url = "https://example.com/reference",
    };
}
