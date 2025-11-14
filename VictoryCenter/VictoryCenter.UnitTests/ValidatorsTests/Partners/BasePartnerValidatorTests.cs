using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class BasePartnerValidatorTests
{
    private readonly TestBasePartnerValidator _validator;
    private readonly string _validDescription;
    private readonly string _tooShortDescription;
    private readonly string _tooLongDescription;

    // Internal class to allow testing the abstract validator
    private class TestBasePartnerValidator : BasePartnerValidator<TestPartnerDto>
    {
    }

    // Internal DTO to satisfy the generic constraint
    private record TestPartnerDto : BasePartnerCreateUpdateDto
    {
    }

    public BasePartnerValidatorTests()
    {
        _validator = new TestBasePartnerValidator();

        // Initialize strings based on constants
        _validDescription = new string('D', PartnerConstants.PartnerDescriptionMinLength);
        _tooShortDescription = new string('D', PartnerConstants.PartnerDescriptionMinLength > 0 ? PartnerConstants.PartnerDescriptionMinLength - 1 : 0);
        _tooLongDescription = new string('D', PartnerConstants.PartnerDescriptionMaxLength + 1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DescriptionIsEmptyOrNull_ShouldHaveError(string description)
    {
        // Arrange
        var model = new TestPartnerDto { Description = description };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BasePartnerCreateUpdateDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        // Arrange
        // Skip test if MinLength is 0 or 1, as "" (NotEmpty) would catch it.
        if (PartnerConstants.PartnerDescriptionMinLength <= 1)
        {
            return;
        }

        var model = new TestPartnerDto { Description = _tooShortDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(BasePartnerCreateUpdateDto.Description), PartnerConstants.PartnerDescriptionMinLength));
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
