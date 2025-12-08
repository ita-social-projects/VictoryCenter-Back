using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class BasePartnerSectionValidatorTests
{
    private readonly TestBasePartnerSectionValidator _validator;

    private readonly string _validTitle;
    private readonly string _tooShortTitle;
    private readonly string _tooLongTitle;

    private readonly string _validDescription;
    private readonly string _tooShortDescription;
    private readonly string _tooLongDescription;

    private class TestBasePartnerSectionValidator : BasePartnerSectionValidator<TestPartnerSectionDto>
    {
    }

    private record TestPartnerSectionDto : BasePartnerSectionCreateUpdateDto
    {
    }

    public BasePartnerSectionValidatorTests()
    {
        _validator = new TestBasePartnerSectionValidator();

        _validTitle = new string('T', PartnerConstants.PartnersSectionTitleMinLength);
        _tooShortTitle = new string('T', PartnerConstants.PartnersSectionTitleMinLength > 0 ?
            PartnerConstants.PartnersSectionTitleMinLength - 1 : 0);
        _tooLongTitle = new string('T', PartnerConstants.PartnersSectionTitleMaxLength + 1);

        _validDescription = new string('D', PartnerConstants.PartnersSectionDescriptionMinLength);
        _tooShortDescription = new string('D', PartnerConstants.PartnersSectionDescriptionMinLength > 0 ?
            PartnerConstants.PartnersSectionDescriptionMinLength - 1 : 0);
        _tooLongDescription = new string('D', PartnerConstants.PartnersSectionDescriptionMaxLength + 1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TitleIsEmptyOrNull_ShouldHaveError(string? title)
    {
        // Arrange
        var model = new TestPartnerSectionDto { Title = title!, Description = _validDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                  nameof(BasePartnerSectionCreateUpdateDto.Title)));
    }

    [Fact]
    public void Validate_TitleIsTooShort_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto
        {
            Title = _tooShortTitle,
            Description = _validDescription
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(BasePartnerSectionCreateUpdateDto.Title),
                    PartnerConstants.PartnersSectionTitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto
        {
            Title = _tooLongTitle,
            Description = _validDescription
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BasePartnerSectionCreateUpdateDto.Title),
                PartnerConstants.PartnersSectionTitleMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DescriptionIsEmptyOrNull_ShouldHaveError(string? description)
    {
        // Arrange
        var model = new TestPartnerSectionDto { Title = _validTitle, Description = description! };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                  nameof(BasePartnerSectionCreateUpdateDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        // Arrange
        var model = new TestPartnerSectionDto { Title = _validTitle, Description = _tooShortDescription };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(BasePartnerSectionCreateUpdateDto.Description),
                    PartnerConstants.PartnersSectionDescriptionMinLength));
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
