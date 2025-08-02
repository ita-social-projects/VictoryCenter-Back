using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Validators.Programs;
using VictoryCenter.BLL.Commands.Programs.Update;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.Programs;

public class UpdateProgramValidatorTests
{
    private UpdateProgramValidator _validator;

    public UpdateProgramValidatorTests()
    {
        _validator = new UpdateProgramValidator(new BaseProgramValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_When_Name_IsNotValid(string? name)
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { Name = name });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_When_Name_IsTooShort(string? name)
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { Name = name });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Name_IsTooLong()
    {
        string name = new string('a', 205);
        var command = new UpdateProgramCommand(new UpdateProgramDto { Name = name });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { Name = "Valid test name" });
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.updateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_When_Description_IsNotValid(string? description)
    {
        var updateProgramDto = new UpdateProgramDto()
        {
            Name = "TestName",
            Status = Status.Published,
            Description = description,
            CategoriesId = [1, 2]
        };
        var command = new UpdateProgramCommand(updateProgramDto);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Description"));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_When_Description_IsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new UpdateProgramCommand(new UpdateProgramDto { Description = description });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Description", ProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Description_IsTooLong()
    {
        var description = new string('a', 1001);
        var command = new UpdateProgramCommand(new UpdateProgramDto { Description = description });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Description", ProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Description_IsValid()
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { Description = "Valid test description!!!" });
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.updateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Categories_AreEmpty()
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { CategoriesId = [] });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.CategoriesId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Categories-list"));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_CategoriesAreNotEmpty()
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { CategoriesId = [1, 2, 3] });
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.updateProgramDto.CategoriesId);
    }
}
