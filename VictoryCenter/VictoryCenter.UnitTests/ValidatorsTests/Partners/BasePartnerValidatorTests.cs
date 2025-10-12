using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class BasePartnerValidatorTests
{
    private readonly TestBasePartnerValidator _validator;
    private readonly string _validDescription;
    private readonly string _tooLongDescription;

    private class TestBasePartnerValidator : BasePartnerValidator<TestPartnerDto>
    {
    }

    private record TestPartnerDto : BasePartnerCreateUpdateDto
    {
    }

    public BasePartnerValidatorTests()
    {
        _validator = new TestBasePartnerValidator();
        _validDescription = "A valid description for a partner.";
        _tooLongDescription = new string('D', PartnerConstants.PartnerDescriptionMaxLength + 1);
    }

    [Fact]
    public void Validate_DescriptionIsEmpty_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerDto { Description = "" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerCreateUpdateDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsNull_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerDto { Description = null! };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerCreateUpdateDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerDto { Description = _tooLongDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerCreateUpdateDto.Description), PartnerConstants.PartnerDescriptionMaxLength));
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new TestPartnerDto { Description = _validDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
