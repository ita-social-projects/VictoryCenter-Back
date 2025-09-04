using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Categories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Categories;
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
    public void Validate_ShouldHaveError_When_Name_IsNotValid(string? name)
    {
        var command = new UpdateCategoryCommand(new UpdateCategoryDto { Name = name! }, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new UpdateCategoryCommand(new UpdateCategoryDto { Name = "Valid Name" }, 1);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UpdateCategoryDto.Name);
    }
}
