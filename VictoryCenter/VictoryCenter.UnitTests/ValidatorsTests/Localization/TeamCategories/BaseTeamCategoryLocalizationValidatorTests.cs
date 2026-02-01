using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Validators.Localization.TeamCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.TeamCategories;
public class BaseTeamCategoryLocalizationValidatorTests
{
    private readonly BaseTeamCategoryLocalizationValidator _validator;

    public BaseTeamCategoryLocalizationValidatorTests()
    {
        _validator = new BaseTeamCategoryLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("e")]
    public void Validate_ShouldHaveError_WhenName_IsInvalid(string? name)
    {
        var model = new CreateTeamCategoryLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = name ?? string.Empty,
            Description = "Valid Description"
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Desc")]
    public void Validate_ShouldHaveError_WhenDescription_IsInvalid(string? description)
    {
        var model = new CreateTeamCategoryLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            Description = description ?? string.Empty
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameAndDescription_IsMaxed()
    {
        var name = new string('a', 21);
        var description = new string('b', 201);
        var model = new CreateTeamCategoryLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = name,
            Description = description
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new CreateTeamCategoryLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            Description = "Valid Description"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
    }
}
