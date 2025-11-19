using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.Validators.Localization.Languages;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.Languages;

public class BaseLocalizationLanguageValidatorTests
{
    private readonly BaseLocalizationLanguageValidator _validator;

    public BaseLocalizationLanguageValidatorTests()
    {
        _validator = new BaseLocalizationLanguageValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("e")]
    [InlineData("eng")]
    public void Validate_ShouldHaveError_When_Code_IsInvalid(string? code)
    {
        var model = new CreateLocalizationLanguageDto { Code = code ?? string.Empty };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("uk")]
    public void Validate_ShouldNotHaveError_When_Code_IsValid(string code)
    {
        var model = new CreateLocalizationLanguageDto { Code = code };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.Code);
    }
}
