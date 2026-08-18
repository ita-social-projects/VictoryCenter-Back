using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateEthicsSectionDtoValidatorTests
{
    private readonly UpdateEthicsSectionDtoValidator _validator = new();

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
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Title)));
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
                  nameof(UpdateEthicsSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.EthicsTitleMaxLength + 1);
        var dto = GetValidDto() with { Title = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateEthicsSectionDto.Title), HippotherapyLandingPageConstants.EthicsTitleMaxLength));
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
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Description)));
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
                  nameof(UpdateEthicsSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.EthicsDescriptionMaxLength + 1);
        var dto = GetValidDto() with { Description = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateEthicsSectionDto.Description), HippotherapyLandingPageConstants.EthicsDescriptionMaxLength));
    }

    [Fact]
    public void Validate_PrinciplesIsNull_ShouldHaveError()
    {
        // Arrange
        var dto = GetValidDto() with { Principles = null! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Principles)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEthicsSectionDto.Principles)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void Validate_PrinciplesCountIsNotExact_ShouldHaveError(int count)
    {
        // Arrange
        var dto = GetValidDto() with { Principles = GetValidPrinciples(count) };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Principles)
              .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainExactlyNItems(
                  nameof(UpdateEthicsSectionDto.Principles), HippotherapyLandingPageConstants.EthicsPrinciplesCount));
    }

    [Fact]
    public void Validate_PrincipleIsTooShort_ShouldHaveError()
    {
        // Arrange
        var principles = GetValidPrinciples(HippotherapyLandingPageConstants.EthicsPrinciplesCount);
        principles[0] = new string('A', HippotherapyLandingPageConstants.TextMinLength - 1);
        var dto = GetValidDto() with { Principles = principles };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor("Principles[0]")
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateEthicsSectionDto.Principles), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_PrincipleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var principles = GetValidPrinciples(HippotherapyLandingPageConstants.EthicsPrinciplesCount);
        principles[0] = new string('A', HippotherapyLandingPageConstants.EthicsPrincipleMaxLength + 1);
        var dto = GetValidDto() with { Principles = principles };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor("Principles[0]")
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateEthicsSectionDto.Principles), HippotherapyLandingPageConstants.EthicsPrincipleMaxLength));
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
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateEthicsSectionDto.ImageId)));
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

    private static List<string> GetValidPrinciples(int count) =>
        Enumerable.Range(0, count)
            .Select(_ => new string('A', HippotherapyLandingPageConstants.TextMinLength))
            .ToList();

    private static UpdateEthicsSectionDto GetValidDto() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        Principles = GetValidPrinciples(HippotherapyLandingPageConstants.EthicsPrinciplesCount),
        ImageId = 1,
    };
}
