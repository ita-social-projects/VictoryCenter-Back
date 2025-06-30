using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Categories.Create;
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WhenNameIsNotValid_ShouldHaveError(string? name)
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = name });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.createCategoryDto.Name)
            .WithErrorMessage("Name can't be empty");
    }

    [Fact]
    public void Validate_WhenNameIsValid_ShouldNotHaveError()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = "Valid Name" });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.createCategoryDto.Name);
    }
}
