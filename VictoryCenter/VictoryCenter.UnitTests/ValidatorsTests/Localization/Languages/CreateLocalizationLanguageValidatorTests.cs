using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.Validators.Localization.Languages;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.Languages;

public class CreateLocalizationLanguageValidatorTests
{
    private readonly CreateLocalizationLanguageValidator _validator;

    public CreateLocalizationLanguageValidatorTests()
    {
        _validator = new CreateLocalizationLanguageValidator(new BaseLocalizationLanguageValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("e")]
    [InlineData("eng")]
    public void Validate_ShouldHaveError_When_Code_IsInvalid(string? code)
    {
        var command = new CreateLocalizationLanguageCommand(
            new CreateLocalizationLanguageDto { Code = code ?? string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateLocalizationLanguageDto.Code);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("es")]
    [InlineData("uk")]
    public void Validate_ShouldNotHaveError_When_Code_IsValid(string code)
    {
        var command = new CreateLocalizationLanguageCommand(
            new CreateLocalizationLanguageDto { Code = code });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.CreateLocalizationLanguageDto.Code);
    }
}
