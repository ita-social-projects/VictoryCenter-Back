using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class UpdatePartnerSectionValidatorTests
{
    private readonly UpdatePartnerSectionValidator _validator;

    private readonly string _validTitle;
    private readonly string _validDescription;
    private readonly List<UpdatePartnerDto> _validPartners;
    private readonly List<UpdatePartnerDto> _tooManyPartners;

    public UpdatePartnerSectionValidatorTests()
    {
        _validator = new UpdatePartnerSectionValidator();
        _validTitle = new string('A', PartnerConstants.TitleMaxLength - 1);
        _validDescription = new string('A', PartnerConstants.DescriptionMaxLength - 1);

        _validPartners = new List<UpdatePartnerDto> { new() };
        _tooManyPartners = Enumerable.Range(0, PartnerConstants.PartnersMaxCount + 1)
                                     .Select(_ => new UpdatePartnerDto())
                                     .ToList();
    }

    [Fact]
    public void Validate_PartnersAreEmpty_ShouldHaveError()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto { Partners = [] };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Partners);
    }

    [Fact]
    public void Validate_PartnersCountIsTooLarge_ShouldHaveError()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto { Partners = _tooManyPartners };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Partners);
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new UpdatePartnersSectionDto
        {
            Title = _validTitle,
            Description = _validDescription,
            Partners = _validPartners,
            PartnerIdsToDelete = []
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
