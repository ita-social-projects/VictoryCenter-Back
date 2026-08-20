using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateTextSectionDtoValidatorTests
{
    private readonly UpdateTextSectionDtoValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TitleIsNullOrEmpty_ShouldHaveError(string? title)
    {
        // Arrange
        var dto = GetValidDto() with { Title = title! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTextSectionDto.Title)));
    }

    [Fact]
    public void Validate_TitleIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShortTitle = new string('A', HippotherapyLandingPageConstants.TitleMinLength - 1);
        var dto = GetValidDto() with { Title = tooShortTitle };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateTextSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongTitle = new string('A', HippotherapyLandingPageConstants.TextSectionTitleMaxLength + 1);
        var dto = GetValidDto() with { Title = tooLongTitle };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateTextSectionDto.Title), HippotherapyLandingPageConstants.TextSectionTitleMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DescriptionIsNullOrEmpty_ShouldHaveError(string? description)
    {
        // Arrange
        var dto = GetValidDto() with { Description = description! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateTextSectionDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShortDescription = new string('A', HippotherapyLandingPageConstants.TextMinLength - 1);
        var dto = GetValidDto() with { Description = tooShortDescription };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateTextSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongDescription = new string('A', HippotherapyLandingPageConstants.TextSectionDescriptionMaxLength + 1);
        var dto = GetValidDto() with { Description = tooLongDescription };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateTextSectionDto.Description), HippotherapyLandingPageConstants.TextSectionDescriptionMaxLength));
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

    private static UpdateTextSectionDto GetValidDto() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
    };
}
