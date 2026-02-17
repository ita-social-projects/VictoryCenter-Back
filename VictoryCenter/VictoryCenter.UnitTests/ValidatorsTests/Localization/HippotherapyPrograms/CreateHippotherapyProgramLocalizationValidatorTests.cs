using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyPrograms;

public class CreateHippotherapyProgramLocalizationValidatorTests
{
    private readonly CreateHippotherapyProgramLocalizationValidator _validator;

    public CreateHippotherapyProgramLocalizationValidatorTests()
    {
        var baseValidator = new BaseHippotherapyProgramLocalizationValidator();
        var contentValidator = new CreateHippotherapyProgramSectionContentLocalizationValidator(
            new BaseProgramSectionContentLocalizationValidator());
        var sectionValidator = new CreateHippotherapyProgramSectionLocalizationValidator(contentValidator);
        _validator = new CreateHippotherapyProgramLocalizationValidator(baseValidator, sectionValidator);
    }

    [Fact]
    public void Validate_ShouldHaveError_When_NameIsTooShort()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 2,
                Name = "A",
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = []
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHippotherapyProgramLocalizationDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramLocalizationDto.Name),
                HippotherapyProgramLocalizationConstants.NameMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_NameIsTooLong()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = new string('A', HippotherapyProgramLocalizationConstants.NameMaxLength + 1),
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = []
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHippotherapyProgramLocalizationDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramLocalizationDto.Name),
                HippotherapyProgramLocalizationConstants.NameMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_DescriptionIsTooShort()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Desc",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = []
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHippotherapyProgramLocalizationDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramLocalizationDto.Description),
                HippotherapyProgramLocalizationConstants.DescriptionMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_DescriptionIsTooLong()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = new string('D', HippotherapyProgramLocalizationConstants.DescriptionMaxLength + 1),
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = []
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHippotherapyProgramLocalizationDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramLocalizationDto.Description),
                HippotherapyProgramLocalizationConstants.DescriptionMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_LocationIsTooShort()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Valid description",
                Location = "L",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = []
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHippotherapyProgramLocalizationDto.Location)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramLocalizationDto.Location),
                HippotherapyProgramLocalizationConstants.LocationMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_ContentTitleIsTooLong()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new()
                            {
                                Title = new string('T', ProgramSectionContentLocalizationConstants.TitleMaxLength + 1)
                            }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(
            "CreateHippotherapyProgramLocalizationDto.Sections[0].Contents[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramSectionContentLocalizationDto.Title),
                ProgramSectionContentLocalizationConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_ContentDescriptionIsTooShort()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new()
                            {
                                Description = "Desc"
                            }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(
            "CreateHippotherapyProgramLocalizationDto.Sections[0].Contents[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramSectionContentLocalizationDto.Description),
                ProgramSectionContentLocalizationConstants.DescriptionMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_ContentAuthorIsTooLong()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new()
                            {
                                Author = new string('A', ProgramSectionContentLocalizationConstants.AuthorMaxLength + 1)
                            }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(
            "CreateHippotherapyProgramLocalizationDto.Sections[0].Contents[0].Author")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramSectionContentLocalizationDto.Author),
                ProgramSectionContentLocalizationConstants.AuthorMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_ContentQuestionIsTooShort()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new()
                            {
                                Question = "Q"
                            }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(
            "CreateHippotherapyProgramLocalizationDto.Sections[0].Contents[0].Question")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramSectionContentLocalizationDto.Question),
                ProgramSectionContentLocalizationConstants.QuestionMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_ContentAnswerIsTooShort()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Name",
                Description = "Valid description",
                Location = "Valid location",
                ParticipantsCount = "10",
                MeetingsCount = "5",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new()
                            {
                                Answer = "Short"
                            }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(
            "CreateHippotherapyProgramLocalizationDto.Sections[0].Contents[0].Answer")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHippotherapyProgramSectionContentLocalizationDto.Answer),
                ProgramSectionContentLocalizationConstants.AnswerMinLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_AllDataIsValid()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Program Name",
                Description = "This is a valid description of the program",
                Location = "Kyiv, Ukraine",
                ParticipantsCount = "20",
                MeetingsCount = "10",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new()
                            {
                                Title = "Valid Title Content"
                            },
                            new()
                            {
                                Description = "This is a valid description for the section content"
                            },
                            new()
                            {
                                Author = "John Doe"
                            },
                            new()
                            {
                                Question = "What is hippotherapy used for?"
                            },
                            new()
                            {
                                Answer = "Hippotherapy is an effective therapy that uses horses to help patients improve their physical and mental health"
                            }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldValidateNestedMultipleSections()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Program Name",
                Description = "This is a valid description of the program",
                Location = "Kyiv, Ukraine",
                ParticipantsCount = "20",
                MeetingsCount = "10",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new() { Title = "Section 1 Title" }
                        }
                    },
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new() { Description = "Section 2 Description with sufficient length" }
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldIgnoreWhitespaceInContentFileds()
    {
        var command = new CreateHippotherapyProgramLocalizationCommand(
            new CreateHippotherapyProgramLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Valid Program Name",
                Description = "This is a valid description of the program",
                Location = "Kyiv, Ukraine",
                ParticipantsCount = "20",
                MeetingsCount = "10",
                Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
                {
                    new()
                    {
                        Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                        {
                            new() { Title = "   " }, // Whitespace only
                            new() { Description = null } // Null values should be allowed
                        }
                    }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
