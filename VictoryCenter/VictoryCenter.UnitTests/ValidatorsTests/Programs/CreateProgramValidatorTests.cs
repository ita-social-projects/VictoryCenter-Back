using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Programs.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.Validators.Programs;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.Programs;

public class CreateProgramValidatorTests
{
    private readonly CreateProgramValidator _validator;

    public CreateProgramValidatorTests()
    {
        _validator = new CreateProgramValidator(new BaseProgramValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = name!,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramDto.Name)));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(string name)
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ProgramDto.Name), ProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', ProgramConstants.MaxNameLength + 1);
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ProgramDto.Name), ProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsNotValid(string? description)
    {
        var createProgramDto = new CreateProgramDto
        {
            Name = "ValidName",
            Status = Status.Published,
            Description = description,
            CategoryIds = [1, 2]
        };
        var command = new CreateProgramCommand(createProgramDto);
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramDto.Description)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new CreateProgramCommand(new CreateProgramDto { Description = description });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ProgramDto.Description), ProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var description = new string('a', ProgramConstants.MaxDescriptionLength + 1);
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = description,
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ProgramDto.Description), ProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsValid()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription!!!",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoriesAreEmpty()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = []
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.CategoryIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramDto.Categories)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCategoriesAreNotEmpty()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2, 3]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.CategoryIds);
    }
}
