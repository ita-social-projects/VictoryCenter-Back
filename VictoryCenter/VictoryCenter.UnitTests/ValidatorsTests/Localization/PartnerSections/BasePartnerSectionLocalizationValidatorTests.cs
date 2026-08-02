using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Validators.Localization.PartnerSections;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnerSections;

public class BasePartnerSectionLocalizationValidatorTests
{
    private readonly BasePartnerSectionLocalizationValidator _validator;

    public BasePartnerSectionLocalizationValidatorTests()
    {
        _validator = new BasePartnerSectionLocalizationValidator(new PartnerLocalizationItemValidator());
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
        var model = ValidModel() with { Title = new string('a', 51) };

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
        var model = ValidModel() with { Description = new string('a', 71) };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPartners_IsNull()
    {
        var model = ValidModel() with { Partners = null! };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Partners);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPartners_ContainDuplicatePartnerIds()
    {
        var model = ValidModel() with
        {
            Partners =
            [
                new UpdatePartnerLocalizationItemDto { PartnerId = 1, Description = "Valid description" },
                new UpdatePartnerLocalizationItemDto { PartnerId = 1, Description = "Another valid description" }
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Partners);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPartners_ContainNullItem()
    {
        var model = ValidModel() with
        {
            Partners =
            [
                null!,
                new UpdatePartnerLocalizationItemDto { PartnerId = 1, Description = "Valid description" }
            ]
        };

        var result = _validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPartnerItem_IsInvalid()
    {
        var model = ValidModel() with
        {
            Partners = [new UpdatePartnerLocalizationItemDto { PartnerId = 0, Description = "" }]
        };

        var result = _validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdatePartnerSectionLocalizationDto ValidModel() => new()
    {
        Title = "Valid section title",
        Description = "Valid section description here",
        Partners = [new UpdatePartnerLocalizationItemDto { PartnerId = 1, Description = "Valid description" }]
    };
}
