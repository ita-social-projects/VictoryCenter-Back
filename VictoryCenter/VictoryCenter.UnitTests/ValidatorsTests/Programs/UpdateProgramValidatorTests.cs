using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Validators.Programs;
using VictoryCenter.BLL.Commands.Programs.Update;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.Programs;

public class UpdateProgramValidatorTests
{
    private readonly UpdateProgramValidator _validator;

    public UpdateProgramValidatorTests()
    {
        _validator = new UpdateProgramValidator(new BaseProgramValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new UpdateProgramCommand(new UpdateProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoriesId = [1, 2] }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramDto.Name)));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(string name)
    {
        var command = new UpdateProgramCommand(
            new UpdateProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoriesId = [1, 2] }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ProgramDto.Name), ProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', ProgramConstants.MaxNameLength + 1);
        var command = new UpdateProgramCommand(
            new UpdateProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoriesId = [1, 2] }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ProgramDto.Name), ProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new UpdateProgramCommand(
            new UpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription",
                Status = Status.Draft,
                CategoriesId = [1, 2]
            }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.updateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsNotValid(string? description)
    {
        var updateProgramDto = new UpdateProgramDto()
        {
            Name = "TestName",
            Status = Status.Published,
            Description = description,
            CategoriesId = [1, 2]
        };
        var command = new UpdateProgramCommand(updateProgramDto, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramDto.Description)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new UpdateProgramCommand(
            new UpdateProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Draft,
                CategoriesId = [1, 2]
            }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ProgramDto.Description), ProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var description = new string('a', ProgramConstants.MaxDescriptionLength + 1);
        var command = new UpdateProgramCommand(
            new UpdateProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Draft,
                CategoriesId = [1, 2]
            }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ProgramDto.Description), ProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsValid()
    {
        var command = new UpdateProgramCommand(
            new UpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoriesId = [1, 2]
            }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.updateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoriesAreEmpty()
    {
        var command = new UpdateProgramCommand(
            new UpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoriesId = []
            }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.updateProgramDto.CategoriesId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramDto.Categories)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCategoriesAreNotEmpty()
    {
        var command = new UpdateProgramCommand(
            new UpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoriesId = [1, 2]
            }, 1);
        TestValidationResult<UpdateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.updateProgramDto.CategoriesId);
    }
}
