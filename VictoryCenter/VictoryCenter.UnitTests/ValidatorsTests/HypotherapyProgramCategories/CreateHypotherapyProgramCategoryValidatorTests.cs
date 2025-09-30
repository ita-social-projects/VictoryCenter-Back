using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.Validators.HypotherapyProgramCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.HypotherapyProgramCategories;

public class CreateHypotherapyProgramCategoryValidatorTests
{
    private readonly CreateHypotherapyProgramCategoryValidator _validatorTests;

    public CreateHypotherapyProgramCategoryValidatorTests()
    {
        _validatorTests = new CreateHypotherapyProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = name });
        TestValidationResult<CreateHypotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.ProgramCategoryDto.Name)
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
        var command = new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = name });
        TestValidationResult<CreateHypotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.ProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Name", HypotherapyProgramCategoryConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HypotherapyProgramCategoryConstants.MaxNameLength + 1);
        var command = new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = name });
        TestValidationResult<CreateHypotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.ProgramCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Name", HypotherapyProgramCategoryConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new CreateHypotherapyProgramCategoryCommand(new CreateHypotherapyProgramCategoryDto { Name = "Valid Name" });
        TestValidationResult<CreateHypotherapyProgramCategoryCommand> result = _validatorTests.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.ProgramCategoryDto.Name);
    }
}
