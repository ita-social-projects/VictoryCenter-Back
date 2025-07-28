using FluentValidation.TestHelper;
using VictoryCenter.BLL.Validators.ProgramCategories;
using VictoryCenter.BLL.Commands.ProgramCategory.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.ProgramCategories;

public class CreateProgramCategoryValidatorTests
{
    private readonly CreateProgramCategoryValidator _validatorTests;

    public CreateProgramCategoryValidatorTests()
    {
        _validatorTests = new CreateProgramCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_When_Name_IsNotValid(string? name)
    {
        var command = new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = name });
        var result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.programCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Validate_ShouldHaveError_When_Name_IsTooShort(int nameLength)
    {
        var name = new string('a', nameLength);
        var command = new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = name });
        var result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.programCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramCategoryConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Name_IsTooLong()
    {
        var name = new string('a', 21);
        var command = new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = name });
        var result = _validatorTests.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.programCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramCategoryConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new CreateProgramCategoryCommand(new CreateProgramCategoryDto { Name = "Valid Name" });
        var result = _validatorTests.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.programCategoryDto.Name);
    }
}
