using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramLocalizationValidatorTests
{
    private readonly UpdateHippotherapyProgramLocalizationValidator _validator;

    public UpdateHippotherapyProgramLocalizationValidatorTests()
    {
        var baseProgramValidator = new BaseHippotherapyProgramLocalizationValidator();
        var baseSectionContentValidator = new BaseProgramSectionContentLocalizationValidator();
        var updateSectionContentValidator = new UpdateHippotherapyProgramSectionContentLocalizationValidator(baseSectionContentValidator);
        var updateSectionValidator = new UpdateHippotherapyProgramSectionLocalizationValidator(updateSectionContentValidator);

        _validator = new UpdateHippotherapyProgramLocalizationValidator(baseProgramValidator, updateSectionValidator);
    }

    [Fact]
    public void Validate_WhenNameIsTooShort_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateHippotherapyProgramLocalizationDto = CreateValidDto() with { Name = "A" }
        };

        var result = _validator.Validate(command);

        Assert.Contains(
            result.Errors,
            x => x.ErrorMessage == ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramLocalizationDto.Name),
                HippotherapyProgramLocalizationConstants.NameMinLength));
    }

    [Fact]
    public void Validate_WhenSectionEntityIdIsInvalid_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateHippotherapyProgramLocalizationDto = CreateValidDto() with
            {
                Sections =
                [
                    new UpdateHippotherapyProgramSectionLocalizationDto
                    {
                        EntityId = 0,
                        Contents = []
                    }

                ]
            }
        };

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "EntityId must be greater than 0.");
    }

    [Fact]
    public void Validate_WhenSectionsIsNull_ShouldNotHaveSectionValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateHippotherapyProgramLocalizationDto = CreateValidDto() with
            {
                Sections = null!
            }
        };

        var result = _validator.Validate(command);

        Assert.DoesNotContain(result.Errors, x => x.ErrorMessage == "EntityId must be greater than 0.");
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        var command = CreateValidCommand();

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    private static UpdateHippotherapyProgramLocalizationCommand CreateValidCommand()
    {
        return new UpdateHippotherapyProgramLocalizationCommand(CreateValidDto(), 1, 1);
    }

    private static UpdateHippotherapyProgramLocalizationDto CreateValidDto()
    {
        return new UpdateHippotherapyProgramLocalizationDto
        {
            Name = "Valid name",
            Description = "Valid description",
            Location = "Kyiv",
            ParticipantsCount = "12",
            MeetingsCount = "5",
            Sections =
            [
                new UpdateHippotherapyProgramSectionLocalizationDto
                {
                    EntityId = 1,
                    Contents =
                    [
                        new UpdateHippotherapyProgramSectionContentLocalizationDto
                        {
                            EntityId = 1,
                            Title = "Valid title"
                        }

                    ]
                }

            ]
        };
    }
}
