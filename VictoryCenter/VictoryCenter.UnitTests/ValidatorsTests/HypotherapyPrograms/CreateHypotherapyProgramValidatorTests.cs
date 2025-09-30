using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.Validators.HypotherapyPrograms;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HypotherapyPrograms;

public class CreateHypotherapyProgramValidatorTests
{
    private readonly CreateHypotherapyProgramValidator _validator;

    public CreateHypotherapyProgramValidatorTests()
    {
        _validator = new CreateHypotherapyProgramValidator(new BaseHypotherapyProgramValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = name!,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Name)));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(string name)
    {
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramDto.Name), HypotherapyProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HypotherapyProgramConstants.MaxNameLength + 1);
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramDto.Name), HypotherapyProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsNotValid(string? description)
    {
        var createProgramDto = new CreateHypotherapyProgramDto
        {
            Name = "ValidName",
            Status = Status.Published,
            Description = description,
            CategoryIds = [1, 2]
        };
        var command = new CreateHypotherapyProgramCommand(createProgramDto);
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Description)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto { Description = description });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramDto.Description), HypotherapyProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var description = new string('a', HypotherapyProgramConstants.MaxDescriptionLength + 1);
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = "ValidName",
            Description = description,
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramDto.Description), HypotherapyProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsValid()
    {
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription!!!",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoriesAreEmpty()
    {
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = []
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.CategoryIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Categories)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCategoriesAreNotEmpty()
    {
        var command = new CreateHypotherapyProgramCommand(new CreateHypotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2, 3]
        });
        TestValidationResult<CreateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.CategoryIds);
    }
}
