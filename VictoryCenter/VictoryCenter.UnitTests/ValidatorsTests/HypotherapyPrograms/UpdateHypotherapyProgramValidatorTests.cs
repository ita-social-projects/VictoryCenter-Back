using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.Validators.HypotherapyPrograms;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HypotherapyPrograms;

public class UpdateHypotherapyProgramValidatorTests
{
    private readonly UpdateHypotherapyProgramValidator _validator;

    public UpdateHypotherapyProgramValidatorTests()
    {
        _validator = new UpdateHypotherapyProgramValidator(new BaseHypotherapyProgramValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new UpdateHypotherapyProgramCommand(new HypotherapyUpdateProgramDto { Name = name!, Description = "ValidDescription", Status = Status.Draft, CategoryIds = [1, 2] }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Name)));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(string name)
    {
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoryIds = [1, 2] }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramDto.Name), HypotherapyProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HypotherapyProgramConstants.MaxNameLength + 1);
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoryIds = [1, 2] }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramDto.Name), HypotherapyProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription",
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.UpdateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsNotValid(string? description)
    {
        var updateProgramDto = new HypotherapyUpdateProgramDto
        {
            Name = "TestName",
            Status = Status.Published,
            Description = description,
            CategoryIds = [1, 2]
        };
        var command = new UpdateHypotherapyProgramCommand(updateProgramDto, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Description)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramDto.Description), HypotherapyProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var description = new string('a', HypotherapyProgramConstants.MaxDescriptionLength + 1);
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramDto.Description), HypotherapyProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsValid()
    {
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.UpdateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoriesAreEmpty()
    {
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = []
            }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.CategoryIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Categories)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCategoriesAreNotEmpty()
    {
        var command = new UpdateHypotherapyProgramCommand(
            new HypotherapyUpdateProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);
        TestValidationResult<UpdateHypotherapyProgramCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(p => p.UpdateProgramDto.CategoryIds);
    }
}
