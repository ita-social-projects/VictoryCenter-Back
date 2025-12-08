using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class UpdatePartnerSectionValidatorTests
{
    private readonly UpdatePartnerSectionValidator _validator;

    private readonly string _validTitle;
    private readonly string _validDescription;
    private readonly CreatePartnerDto _validCreatePartnerDto;
    private readonly UpdatePartnerDto _validUpdatePartnerDto;

    public UpdatePartnerSectionValidatorTests()
    {
        _validator = new UpdatePartnerSectionValidator();
        _validTitle = "A valid title";
        _validDescription = "A valid description for the section.";
        _validCreatePartnerDto = new CreatePartnerDto { Description = "New Partner", ImageId = 1 };
        _validUpdatePartnerDto = new UpdatePartnerDto { Id = 1, Description = "Updated Partner", ImageId = 2 };
    }

    [Fact]
    public void Validate_WithValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnersToCreate = [_validCreatePartnerDto],
            PartnersToUpdate = [_validUpdatePartnerDto],
            PartnerIdsToDelete = [1L]
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyLists_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnersToCreate = [],
            PartnersToUpdate = [],
            PartnerIdsToDelete = []
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PartnersToCreateIsNull_ShouldHaveError()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnersToCreate = null!
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnersToCreate);
    }

    [Fact]
    public void Validate_PartnersToUpdateIsNull_ShouldHaveError()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnersToUpdate = null!
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnersToUpdate);
    }

    [Fact]
    public void Validate_PartnerIdsToDeleteIsNull_ShouldHaveError()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnerIdsToDelete = null!
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnerIdsToDelete);
    }

    [Fact]
    public void Validate_PartnersToCreateCountIsTooLarge_ShouldHaveError()
    {
        // Arrange
        var tooManyPartners = Enumerable.Range(0, PartnerConstants.PartnersSectionPartnersMaxCount + 1)
            .Select(_ => _validCreatePartnerDto).ToList();
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnersToCreate = tooManyPartners
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnersToCreate);
    }

    [Fact]
    public void Validate_PartnersToUpdateCountIsTooLarge_ShouldHaveError()
    {
        // Arrange
        var tooManyPartners = Enumerable.Range(0, PartnerConstants.PartnersSectionPartnersMaxCount + 1)
            .Select(_ => _validUpdatePartnerDto).ToList();
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnersToUpdate = tooManyPartners
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnersToUpdate);
    }

    [Fact]
    public void Validate_PartnerIdsToDeleteCountIsTooLarge_ShouldHaveError()
    {
        // Arrange
        var tooManyIds = Enumerable.Range(0, PartnerConstants.PartnersSectionPartnersMaxCount + 1)
            .Select(i => (long)i).ToList();
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            PartnerIdsToDelete = tooManyIds
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PartnerIdsToDelete);
    }
}
