using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Categories.CreateCategory;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Validators.Categories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Categories;

public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _validator = new CreateCategoryValidator();
    }
    
    [Fact]
    public void Validate_ShouldHaveError_When_Name_IsEmpty()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.createCategoryDto.Name)
            .WithErrorMessage("Name can't be empty");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = "Valid Name" });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.createCategoryDto.Name);
    }
}
