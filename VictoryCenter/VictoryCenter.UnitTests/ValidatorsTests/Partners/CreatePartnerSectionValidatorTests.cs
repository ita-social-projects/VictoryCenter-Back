using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class CreatePartnerSectionValidatorTests
{
    private readonly CreatePartnerSectionValidator _validator;

    private readonly string _validTitle;
    private readonly string _validDescription;
    private readonly List<CreatePartnerDto> _validPartners;
    private readonly List<CreatePartnerDto> _tooManyPartners;

    public CreatePartnerSectionValidatorTests()
    {
        _validator = new CreatePartnerSectionValidator();
        _validTitle = new string('A', PartnerConstants.PartnersSectionTitleMaxLength - 1);
        _validDescription = new string('A', PartnerConstants.PartnersSectionDescriptionMaxLength - 1);

        _validPartners = [
            new CreatePartnerDto
            {
                Description = "Valid Partner Description",
                ImageId = 1
            }

        ];

        _tooManyPartners = Enumerable.Range(0, PartnerConstants.PartnersSectionPartnersMaxCount + 1)
                                     .Select(_ => new CreatePartnerDto())
                                     .ToList();
    }

    [Fact]
    public void Validate_PartnersAreEmpty_ShouldHaveError()
    {
        // Arrange
        var model = new CreatePartnersSectionDto { Partners = new List<CreatePartnerDto>() };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Partners);
    }

    [Fact]
    public void Validate_PartnersCountIsTooLarge_ShouldHaveError()
    {
        // Arrange
        var model = new CreatePartnersSectionDto { Partners = _tooManyPartners };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Partners);
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new CreatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            Partners = _validPartners
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
