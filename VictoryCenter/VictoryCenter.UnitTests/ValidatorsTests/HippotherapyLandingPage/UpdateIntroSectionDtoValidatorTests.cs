using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateIntroSectionDtoValidatorTests
{
    private readonly UpdateIntroSectionDtoValidator _validator = new();

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
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateIntroSectionDto.Title)));
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
                  nameof(UpdateIntroSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongTitle = new string('A', HippotherapyLandingPageConstants.IntroTitleMaxLength + 1);
        var dto = GetValidDto() with { Title = tooLongTitle };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateIntroSectionDto.Title), HippotherapyLandingPageConstants.IntroTitleMaxLength));
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
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateIntroSectionDto.Description)));
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
                  nameof(UpdateIntroSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongDescription = new string('A', HippotherapyLandingPageConstants.IntroDescriptionMaxLength + 1);
        var dto = GetValidDto() with { Description = tooLongDescription };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateIntroSectionDto.Description), HippotherapyLandingPageConstants.IntroDescriptionMaxLength));
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
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateIntroSectionDto.ImageId)));
    }

    [Fact]
    public void Validate_ImageIdIsNull_ShouldNotHaveError()
    {
        // Arrange
        var dto = GetValidDto() with { ImageId = null };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ImageId);
    }

    [Fact]
    public void Validate_TitleWithHtmlFormattingWithinVisibleLimit_ShouldNotHaveError()
    {
        // Arrange
        var visibleText = new string('A', HippotherapyLandingPageConstants.IntroTitleMaxLength);
        var dto = GetValidDto() with { Title = $"<p><strong>{visibleText}</strong></p>" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_TitleWithHtmlPaddingExceedingVisibleLimit_ShouldHaveError()
    {
        // Arrange
        var visibleText = new string('A', HippotherapyLandingPageConstants.IntroTitleMaxLength + 1);
        var dto = GetValidDto() with { Title = $"<p><strong>{visibleText}</strong></p>" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateIntroSectionDto.Title), HippotherapyLandingPageConstants.IntroTitleMaxLength));
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

    private static UpdateIntroSectionDto GetValidDto() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        ImageId = 1,
    };
}
