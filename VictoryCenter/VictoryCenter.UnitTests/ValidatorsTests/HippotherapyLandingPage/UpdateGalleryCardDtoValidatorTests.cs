using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateGalleryCardDtoValidatorTests
{
    private readonly UpdateGalleryCardDtoValidator _validator = new();

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
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateGalleryCardDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.TextMinLength - 1);
        var dto = GetValidDto() with { Description = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateGalleryCardDto.Description), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.GalleryCardDescriptionMaxLength + 1);
        var dto = GetValidDto() with { Description = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateGalleryCardDto.Description), HippotherapyLandingPageConstants.GalleryCardDescriptionMaxLength));
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
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateGalleryCardDto.ImageId)));
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
    public void Validate_ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = GetValidDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateGalleryCardDto GetValidDto() => new()
    {
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        ImageId = 1,
    };
}
