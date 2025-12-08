using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class ReorderPartnersValidatorTests
{
    private readonly ReorderPartnersValidator _validator = new();

    [Fact]
    public void Validate_PartnersSectionIdIsInvalid_ShouldHaveError()
    {
        // Arrange
        var model = new ReorderPartnersDto
        {
            PartnersSectionId = -1,
            OrderedIds = new List<long> { 1, 2 }
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnersSectionId);
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new ReorderPartnersDto
        {
            PartnersSectionId = 1,
            OrderedIds = new List<long> { 3, 1, 2 }
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
