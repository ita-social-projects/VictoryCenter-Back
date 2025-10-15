using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.Validators.HippotherapyProgramCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyProgramCategories;

public class CreateHippotherapyProgramCategoryValidatorTests
{
    private readonly CreateHippotherapyProgramCategoryValidator _validatorTests;

    public CreateHippotherapyProgramCategoryValidatorTests()
    {
        _validatorTests = new CreateHippotherapyProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = name });
        TestValidationResult<CreateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.CreateProgramCategoryDto.Name)
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
        var command = new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = name });
        TestValidationResult<CreateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.CreateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Name", HippotherapyProgramCategoryConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HippotherapyProgramCategoryConstants.MaxNameLength + 1);
        var command = new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = name });
        TestValidationResult<CreateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.CreateProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Name", HippotherapyProgramCategoryConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new CreateHippotherapyProgramCategoryCommand(new CreateHippotherapyProgramCategoryDto { Name = "Valid Name" });
        TestValidationResult<CreateHippotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.CreateProgramCategoryDto.Name);
    }
}
