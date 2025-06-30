using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Categories.Update;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Validators.Categories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Categories;

public class UpdateCategoryValidatorTests
{
    private readonly UpdateCategoryValidator _validator;

    public UpdateCategoryValidatorTests()
    {
        _validator = new UpdateCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WhenNameIsNotValid_ShouldHaveError(string? name)
    {
        var command = new UpdateCategoryCommand(new UpdateCategoryDto { Name = name });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.updateCategoryDto.Name)
            .WithErrorMessage("Name can't be empty");
    }

    [Fact]
    public void Validate_WhenNameIsValid_ShouldNotHaveError()
    {
        var command = new UpdateCategoryCommand(new UpdateCategoryDto { Name = "Valid Name" });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.updateCategoryDto.Name);
    }
}
