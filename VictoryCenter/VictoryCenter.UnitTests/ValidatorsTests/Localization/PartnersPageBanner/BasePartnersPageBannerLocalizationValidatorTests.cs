using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnersPageBanner;

public class BasePartnersPageBannerLocalizationValidatorTests
{
    private readonly BasePartnersPageBannerLocalizationValidator _validator;

    public BasePartnersPageBannerLocalizationValidatorTests()
    {
        _validator = new BasePartnersPageBannerLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("short")]
    public void Validate_ShouldHaveError_WhenTitle_IsInvalid(string? title)
    {
        var model = ValidModel() with { Title = title! };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsTooLong()
    {
        var model = ValidModel() with { Title = new string('a', 31) };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("short")]
    public void Validate_ShouldHaveError_WhenDescription_IsInvalid(string? description)
    {
        var model = ValidModel() with { Description = description! };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var model = ValidModel() with { Description = new string('a', 31) };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdatePartnersPageBannerLocalizationDto ValidModel() => new()
    {
        Title = "Valid banner title",
        Description = "Valid banner description"
    };
}
