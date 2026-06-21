using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.BLL.Validators.Localization.History;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.History;

public class BaseHistorySectionContentLocalizationValidatorTests
{
    private readonly BaseHistorySectionContentLocalizationValidator _validator;

    public BaseHistorySectionContentLocalizationValidatorTests()
    {
        _validator = new BaseHistorySectionContentLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenTitleIsNullOrEmpty_ShouldNotHaveValidationError(string? title)
    {
        // Arrange
        var model = new CreateHistorySectionContentLocalizationDto { Title = title };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var validTitle = new string('a', HistoryLocalizationConstants.ContentTitleMinLength + 1);
        var model = new CreateHistorySectionContentLocalizationDto { Title = validTitle };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleIsTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var invalidTitle = new string('a', Math.Max(1, HistoryLocalizationConstants.ContentTitleMinLength - 1));
        var model = new CreateHistorySectionContentLocalizationDto { Title = invalidTitle };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                  nameof(CreateHistorySectionContentLocalizationDto.Title),
                  HistoryLocalizationConstants.ContentTitleMinLength));
    }

    [Fact]
    public void Validate_WhenTitleIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var invalidTitle = new string('a', HistoryLocalizationConstants.ContentTitleMaxLength + 1);
        var model = new CreateHistorySectionContentLocalizationDto { Title = invalidTitle };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                  nameof(CreateHistorySectionContentLocalizationDto.Title),
                  HistoryLocalizationConstants.ContentTitleMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenDescriptionIsNullOrEmpty_ShouldNotHaveValidationError(string? description)
    {
        // Arrange
        var model = new CreateHistorySectionContentLocalizationDto { Description = description };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WhenDescriptionIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var validDescription = new string('a', HistoryLocalizationConstants.ContentDescriptionMinLength + 1);
        var model = new CreateHistorySectionContentLocalizationDto { Description = validDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WhenDescriptionIsTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var invalidDescription = new string('a', Math.Max(1, HistoryLocalizationConstants.ContentDescriptionMinLength - 1));
        var model = new CreateHistorySectionContentLocalizationDto { Description = invalidDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                  nameof(CreateHistorySectionContentLocalizationDto.Description),
                  HistoryLocalizationConstants.ContentDescriptionMinLength));
    }

    [Fact]
    public void Validate_WhenDescriptionIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var invalidDescription = new string('a', HistoryLocalizationConstants.ContentDescriptionMaxLength + 1);
        var model = new CreateHistorySectionContentLocalizationDto { Description = invalidDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                  nameof(CreateHistorySectionContentLocalizationDto.Description),
                  HistoryLocalizationConstants.ContentDescriptionMaxLength));
    }
}
