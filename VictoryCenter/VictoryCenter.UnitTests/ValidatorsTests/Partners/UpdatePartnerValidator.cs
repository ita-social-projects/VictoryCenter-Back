using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class UpdatePartnerValidatorTests
{
    private readonly UpdatePartnerValidator _validator = new();

    [Fact]
    public void Validate_IdIsInvalid_ShouldHaveError()
    {
        // Arrange
        var model = new UpdatePartnerDto
        {
            Id = -1,
            Description = "Valid Description"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new UpdatePartnerDto
        {
            Id = 1,
            Description = "Valid Description"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
