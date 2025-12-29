using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Validators.HippotherapyPrograms;
using VictoryCenter.BLL.Validators.HippotherapyProgramSections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyPrograms;

public class UpdateHippotherapyProgramValidatorTests
{
    private readonly UpdateHippotherapyProgramValidator _validator;

    public UpdateHippotherapyProgramValidatorTests()
    {
        _validator = new UpdateHippotherapyProgramValidator(
            new BaseHippotherapyProgramValidator(new BaseHippotherapyProgramSectionValidator()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto { Name = name!, Description = "ValidDescription", Status = Status.Draft, CategoryIds = [1, 2] }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Name)));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(string name)
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoryIds = [1, 2] }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.Name), HippotherapyProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HippotherapyProgramConstants.MaxNameLength + 1);
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto { Name = name, Description = "ValidDescription", Status = Status.Draft, CategoryIds = [1, 2] }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.Name), HippotherapyProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription",
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(p => p.UpdateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsNotValid(string? description)
    {
        var updateProgramDto = new UpdateHippotherapyProgramDto
        {
            Name = "TestName",
            Status = Status.Published,
            Description = description,
            CategoryIds = [1, 2],

            BackgroundImageId = 1,
            PreviewImageId = 1
        };

        var command = new UpdateHippotherapyProgramCommand(updateProgramDto, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Description)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Published,
                CategoryIds = [1, 2],

                BackgroundImageId = 1,
                PreviewImageId = 1
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.Description), HippotherapyProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var description = new string('a', HippotherapyProgramConstants.MaxDescriptionLength + 1);
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.Description), HippotherapyProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsValid()
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(p => p.UpdateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoriesAreEmpty()
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = []
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.UpdateProgramDto.CategoryIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Categories)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCategoriesAreNotEmpty()
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = [1, 2]
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(p => p.UpdateProgramDto.CategoryIds);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSectionTemplateIsInvalid()
    {
        var command = new UpdateHippotherapyProgramCommand(
            new UpdateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = "ValidDescription!!!",
                Status = Status.Draft,
                CategoryIds = [1, 2],
                Sections =
                [
                    new CreateHippotherapyProgramSectionDto
                    {
                        Template = (ProgramSectionTemplate)999
                    }

                ]
            }, 1);

        TestValidationResult<UpdateHippotherapyProgramCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("UpdateProgramDto.Sections[0].Template")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(CreateHippotherapyProgramSectionDto.Template)));
    }
}
