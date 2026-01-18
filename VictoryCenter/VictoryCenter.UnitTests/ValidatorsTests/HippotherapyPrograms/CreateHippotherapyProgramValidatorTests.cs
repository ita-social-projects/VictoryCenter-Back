using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Validators.HippotherapyPrograms;
using VictoryCenter.BLL.Validators.HippotherapyProgramSections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyPrograms;

public class CreateHippotherapyProgramValidatorTests
{
    private readonly CreateHippotherapyProgramValidator _validator;

    public CreateHippotherapyProgramValidatorTests()
    {
        _validator = new CreateHippotherapyProgramValidator(
            new BaseHippotherapyProgramValidator(new BaseHippotherapyProgramSectionValidator()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsNotValid(string? name)
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = name!,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Name)));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("te")]
    [InlineData("tes")]
    [InlineData("test")]
    public void Validate_ShouldHaveError_WhenNameIsTooShort(string name)
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(HippotherapyProgramDto.Name),
                    HippotherapyProgramConstants.MinNameLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var name = new string('a', HippotherapyProgramConstants.MaxNameLength + 1);

        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = name,
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Name)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(HippotherapyProgramDto.Name),
                    HippotherapyProgramConstants.MaxNameLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsNotValid(string? description)
    {
        var createProgramDto = new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Status = Status.Published,
            Description = description,
            CategoryIds = [1, 2],
            BackgroundImageId = 1,
            PreviewImageId = 1
        };

        var command = new CreateHippotherapyProgramCommand(createProgramDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Description)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(int descriptionLength)
    {
        var description = new string('a', descriptionLength);

        var command = new CreateHippotherapyProgramCommand(
            new CreateHippotherapyProgramDto
            {
                Name = "ValidName",
                Description = description,
                Status = Status.Published,
                CategoryIds = [1, 2],
                BackgroundImageId = 1,
                PreviewImageId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(HippotherapyProgramDto.Description),
                HippotherapyProgramConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var description = new string('a', HippotherapyProgramConstants.MaxDescriptionLength + 1);

        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Description = description,
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(HippotherapyProgramDto.Description),
                HippotherapyProgramConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionIsValid()
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription!!!",
            Status = Status.Draft,
            CategoryIds = [1, 2]
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoriesAreEmpty()
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = []
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(p => p.CreateProgramDto.CategoryIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Categories)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCategoriesAreNotEmpty()
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2, 3]
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(p => p.CreateProgramDto.CategoryIds);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSectionIsNotValid()
    {
        var command = new CreateHippotherapyProgramCommand(new CreateHippotherapyProgramDto
        {
            Name = "ValidName",
            Description = "ValidProgramDescription",
            Status = Status.Draft,
            CategoryIds = [1, 2],
            Sections =
            [
                new CreateHippotherapyProgramSectionDto
                {
                    Template = ProgramSectionTemplate.TextOnly,
                    Order = -1,
                    Contents =
                    [
                        new CreateProgramSectionContentDto
                        {
                            ContentType = ContentType.Title,
                            Order = 0,
                            Title = "ValidTitle"
                        },
                        new CreateProgramSectionContentDto
                        {
                            ContentType = ContentType.Description,
                            Order = 1,
                            Description = "ValidDescription"
                        }

                    ]
                }

            ]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateProgramDto.Sections[0].Order")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(
                nameof(CreateHippotherapyProgramSectionDto.Order), -1));
    }
}
