using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Validators.Localization.PartnerSections;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.PartnerSections;

public class PartnerLocalizationItemValidatorTests
{
    private readonly PartnerLocalizationItemValidator _validator;

    public PartnerLocalizationItemValidatorTests()
    {
        _validator = new PartnerLocalizationItemValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenPartnerId_IsNotPositive(long partnerId)
    {
        var model = new UpdatePartnerLocalizationItemDto
        {
            PartnerId = partnerId,
            Description = "Valid description"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PartnerId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc")]
    public void Validate_ShouldHaveError_WhenDescription_IsInvalid(string? description)
    {
        var model = new UpdatePartnerLocalizationItemDto
        {
            PartnerId = 1,
            Description = description!
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var model = new UpdatePartnerLocalizationItemDto
        {
            PartnerId = 1,
            Description = new string('a', 51)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdatePartnerLocalizationItemDto
        {
            PartnerId = 1,
            Description = "Valid description"
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.PartnerId);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
