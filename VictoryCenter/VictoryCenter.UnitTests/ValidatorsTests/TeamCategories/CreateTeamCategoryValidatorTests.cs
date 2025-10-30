using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Validators.TeamCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamCategories;

public class CreateTeamCategoryValidatorTests
{
    private readonly CreateTeamCategoryValidator _validator;

    public CreateTeamCategoryValidatorTests()
    {
        _validator = new CreateTeamCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_When_Name_IsNotValid(string? name)
    {
        var command = new CreateTeamCategoryCommand(new CreateTeamCategoryDto { Name = name! });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new CreateTeamCategoryCommand(new CreateTeamCategoryDto { Name = "Valid Name" });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.CreateCategoryDto.Name);
    }
}
