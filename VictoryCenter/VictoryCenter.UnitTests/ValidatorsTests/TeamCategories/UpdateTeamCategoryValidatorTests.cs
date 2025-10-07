using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Validators.TeamCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamCategories;

public class UpdateTeamCategoryValidatorTests
{
    private readonly UpdateTeamCategoryValidator _validator;

    public UpdateTeamCategoryValidatorTests()
    {
        _validator = new UpdateTeamCategoryValidator(new BaseTeamCategoryValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_When_Name_IsNotValid(string? name)
    {
        var command = new UpdateTeamCategoryCommand(new UpdateTeamCategoryDto { Name = name! }, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateTeamCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new UpdateTeamCategoryCommand(new UpdateTeamCategoryDto { Name = "Valid Name" }, 1);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UpdateTeamCategoryDto.Name);
    }
}
