using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.Validators.Localization.Languages;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.Languages;

public class UpdateLocalizationLanguageValidatorTests
{
    private readonly UpdateLocalizationLanguageValidator _validator;

    public UpdateLocalizationLanguageValidatorTests()
    {
        _validator = new UpdateLocalizationLanguageValidator(new BaseLocalizationLanguageValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("e")]
    [InlineData("eng")]
    public void Validate_ShouldHaveError_When_Code_IsInvalid(string? code)
    {
        // Arrange
        var command = new UpdateLocalizationLanguageCommand(
            new UpdateLocalizationLanguageDto { Code = code ?? string.Empty }, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateLocalizationLanguageDto.Code);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("es")]
    [InlineData("uk")]
    public void Validate_ShouldNotHaveError_When_Code_IsValid(string code)
    {
        // Arrange
        var command = new UpdateLocalizationLanguageCommand(
            new UpdateLocalizationLanguageDto { Code = code }, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.UpdateLocalizationLanguageDto.Code);
    }
}
