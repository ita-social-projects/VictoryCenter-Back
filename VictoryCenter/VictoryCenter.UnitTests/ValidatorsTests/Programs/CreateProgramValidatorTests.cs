using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Validators.Programs;
using VictoryCenter.BLL.Commands.Programs.Create;
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
    public void Validate_ShouldHaveError_When_Name_IsNotValid(string? name)
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoriesId = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_When_Name_IsTooShort(string name)
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoriesId = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Name_IsTooLong()
    {
        var name = new string('a', ProgramConstants.MaxNameLength + 1);
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoriesId = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Name_IsValid()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoriesId = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.createProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_When_Description_IsNotValid(string? description)
    {
        var createProgramDto = new CreateProgramDto()
        {
            Name = "ValidName",
            Status = Status.Published,
            Description = description,
            CategoriesId = [1, 2]
        };
        var command = new CreateProgramCommand(createProgramDto);
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Description"));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_When_Description_IsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new CreateProgramCommand(new CreateProgramDto { Description = description });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Description", ProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Description_IsTooLong()
    {
        var description = new string('a', ProgramConstants.MaxDescriptionLength + 1);
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = description,
            Status = Status.Draft,
            CategoriesId = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Description", ProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_Description_IsValid()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription!!!",
            Status = Status.Draft,
            CategoriesId = [1, 2]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.createProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_When_Categories_AreEmpty()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoriesId = []
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.createProgramDto.CategoriesId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired("Categories-list"));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_CategoriesAreNotEmpty()
    {
        var command = new CreateProgramCommand(new CreateProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoriesId = [1, 2, 3]
        });
        TestValidationResult<CreateProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.createProgramDto.CategoriesId);
    }
}
