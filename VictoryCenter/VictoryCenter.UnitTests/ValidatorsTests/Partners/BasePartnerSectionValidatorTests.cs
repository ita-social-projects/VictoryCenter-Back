using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class BasePartnerSectionValidatorTests
{
    private readonly TestBasePartnerSectionValidator _validator;

    private readonly string _validTitle;
    private readonly string _tooLongTitle;
    private readonly string _validDescription;
    private readonly string _tooLongDescription;
    private readonly List<TestPartnerDto> _validPartners;
    private readonly List<TestPartnerDto> _tooManyPartners;

    private class TestBasePartnerSectionValidator : BasePartnerSectionValidator<TestPartnerSectionDto>
    {
    }

    private record TestPartnerDto : BasePartnerCreateUpdateDto
    {
    }

    private record TestPartnerSectionDto : BasePartnerSectionCreateUpdateDto
    {
    }

    public BasePartnerSectionValidatorTests()
    {
        _validator = new TestBasePartnerSectionValidator();

        _validTitle = "Valid Title";
        _tooLongTitle = new string('T', PartnerConstants.TitleMaxLength + 1);

        _validDescription = "A valid section description.";
        _tooLongDescription = new string('D', PartnerConstants.DescriptionMaxLength + 1);

        _validPartners = new List<TestPartnerDto> { new() };
        _tooManyPartners = Enumerable.Range(0, PartnerConstants.PartnersMaxCount + 1)
                                     .Select(_ => new TestPartnerDto())
                                     .ToList();
    }

    [Fact]
    public void Validate_TitleIsEmpty_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto { Title = "" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto { Title = _tooLongTitle };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_DescriptionIsEmpty_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto { Description = "" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto { Description = _tooLongDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new TestPartnerSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
