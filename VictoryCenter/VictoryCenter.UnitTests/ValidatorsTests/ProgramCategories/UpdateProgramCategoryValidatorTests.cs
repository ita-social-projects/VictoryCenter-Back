using FluentValidation.TestHelper;
using VictoryCenter.BLL.Validators.ProgramCategories;
using VictoryCenter.BLL.Commands.ProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.ProgramCategories;
namespace VictoryCenter.UnitTests.ValidatorsTests.ProgramCategories;

public class UpdateProgramCategoryValidatorTests
{
    private readonly UpdateProgramCategoryValidator _validatorTests;

    public UpdateProgramCategoryValidatorTests()
    {
        _validatorTests = new UpdateProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new UpdateProgramCategoryCommand(new UpdateProgramCategoryDto { Name = name }, 1);
        TestValidationResult<UpdateProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.updateProgramCategoryDto.Name)
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
        var command = new UpdateProgramCategoryCommand(new UpdateProgramCategoryDto { Name = name }, 1);
        TestValidationResult<UpdateProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.updateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramCategoryConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', ProgramCategoryConstants.MaxNameLength + 1);
        var command = new UpdateProgramCategoryCommand(new UpdateProgramCategoryDto { Name = name }, 1);
        TestValidationResult<UpdateProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.updateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramCategoryConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new UpdateProgramCategoryCommand(new UpdateProgramCategoryDto { Name = "Valid Name" }, 1);
        TestValidationResult<UpdateProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.updateProgramCategoryDto.Name);
    }
}
