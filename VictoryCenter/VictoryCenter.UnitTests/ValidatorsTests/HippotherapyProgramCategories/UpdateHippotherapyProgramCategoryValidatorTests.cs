using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.Validators.HippotherapyProgramCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyProgramCategories;

public class UpdateHippotherapyProgramCategoryValidatorTests
{
    private readonly UpdateHippotherapyProgramCategoryValidator _validatorTests;

    public UpdateHippotherapyProgramCategoryValidatorTests()
    {
        _validatorTests = new UpdateHippotherapyProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new UpdateHippotherapyProgramCategoryCommand(new UpdateHippotherapyProgramCategoryDto { Name = name! }, 1);
        TestValidationResult<UpdateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.UpdateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(int nameLength)
    {
        var name = new string('a', nameLength);
        var command = new UpdateHippotherapyProgramCategoryCommand(new UpdateHippotherapyProgramCategoryDto { Name = name }, 1);
        TestValidationResult<UpdateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.UpdateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Name", HippotherapyProgramCategoryConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HippotherapyProgramCategoryConstants.MaxNameLength + 1);
        var command = new UpdateHippotherapyProgramCategoryCommand(new UpdateHippotherapyProgramCategoryDto { Name = name }, 1);
        TestValidationResult<UpdateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.UpdateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Name", HippotherapyProgramCategoryConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new UpdateHippotherapyProgramCategoryCommand(new UpdateHippotherapyProgramCategoryDto { Name = "Valid Name" }, 1);
        TestValidationResult<UpdateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.UpdateProgramCategoryDto.Name);
    }
}
