using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateGallerySectionDtoValidatorTests
{
    private readonly UpdateGallerySectionDtoValidator _validator = new(new UpdateGalleryCardDtoValidator());

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
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateGallerySectionDto.Title)));
    }

    [Fact]
    public void Validate_TitleIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.TitleMinLength - 1);
        var dto = GetValidDto() with { Title = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateGallerySectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.GalleryTitleMaxLength + 1);
        var dto = GetValidDto() with { Title = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateGallerySectionDto.Title), HippotherapyLandingPageConstants.GalleryTitleMaxLength));
    }

    [Fact]
    public void Validate_CardsIsNull_ShouldHaveError()
    {
        // Arrange
        var dto = GetValidDto() with { Cards = null! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cards)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateGallerySectionDto.Cards)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void Validate_CardsCountIsNotExact_ShouldHaveError(int count)
    {
        // Arrange
        var dto = GetValidDto() with { Cards = GetValidCards(count) };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cards)
              .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainExactlyNItems(
                  nameof(UpdateGallerySectionDto.Cards), HippotherapyLandingPageConstants.GalleryCardsCount));
    }

    [Fact]
    public void Validate_CardHasInvalidDescription_ShouldHaveError()
    {
        // Arrange
        var cards = GetValidCards(HippotherapyLandingPageConstants.GalleryCardsCount);
        cards[0] = cards[0] with { Description = string.Empty };
        var dto = GetValidDto() with { Cards = cards };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor("Cards[0].Description");
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

    private static List<UpdateGalleryCardDto> GetValidCards(int count) =>
        Enumerable.Range(0, count)
            .Select(_ => new UpdateGalleryCardDto
            {
                Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
                ImageId = 1,
            })
            .ToList();

    private static UpdateGallerySectionDto GetValidDto() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Cards = GetValidCards(HippotherapyLandingPageConstants.GalleryCardsCount),
    };
}
