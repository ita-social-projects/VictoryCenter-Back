using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyPrograms;

public class BaseHippotherapyProgramLocalizationValidatorTests
{
    private readonly BaseHippotherapyProgramLocalizationValidator _validator;

    public BaseHippotherapyProgramLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyProgramLocalizationValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Validate_ShouldHaveError_WhenName_IsInvalid(string? name)
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = name!,
            Description = "Valid Description"
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_IsTooLong()
    {
        var name = new string('a', 201);
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = name,
            Description = "Valid Description"
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Short")]
    public void Validate_ShouldHaveError_WhenDescription_IsInvalid(string? description)
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            Description = description!
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var description = new string('b', 2001);
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            Description = description
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("L")]
    public void Validate_ShouldHaveError_WhenLocation_IsInvalid(string? location)
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            Location = location!
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Location);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenLocation_IsTooLong()
    {
        var location = new string('c', 201);
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            Location = location
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Location);
    }

    [Theory]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenParticipantsCount_IsInvalid(string? participantsCount)
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            ParticipantsCount = participantsCount!
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.ParticipantsCount);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenParticipantsCount_IsTooLong()
    {
        var participantsCount = new string('d', 101);
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            ParticipantsCount = participantsCount
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.ParticipantsCount);
    }

    [Theory]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenMeetingsCount_IsInvalid(string? meetingsCount)
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            MeetingsCount = meetingsCount!
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.MeetingsCount);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMeetingsCount_IsTooLong()
    {
        var meetingsCount = new string('e', 101);
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Name",
            MeetingsCount = meetingsCount
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.MeetingsCount);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAllFields_AreMaxed()
    {
        var name = new string('a', 201);
        var description = new string('b', 2001);
        var location = new string('c', 201);
        var participantsCount = new string('d', 101);
        var meetingsCount = new string('e', 101);

        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = name,
            Description = description,
            Location = location,
            ParticipantsCount = participantsCount,
            MeetingsCount = meetingsCount
        };
        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Description);
        result.ShouldHaveValidationErrorFor(c => c.Location);
        result.ShouldHaveValidationErrorFor(c => c.ParticipantsCount);
        result.ShouldHaveValidationErrorFor(c => c.MeetingsCount);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Valid Program Name",
            Description = "This is a valid and sufficiently long description.",
            Location = "Kyiv, Ukraine",
            ParticipantsCount = "10-15",
            MeetingsCount = "Twice a week"
        };
        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
        result.ShouldNotHaveValidationErrorFor(c => c.Location);
        result.ShouldNotHaveValidationErrorFor(c => c.ParticipantsCount);
        result.ShouldNotHaveValidationErrorFor(c => c.MeetingsCount);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenOptionalFields_AreNull()
    {
        var model = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = null,
            Description = null,
            Location = null,
            ParticipantsCount = null,
            MeetingsCount = null
        };
        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
        result.ShouldNotHaveValidationErrorFor(c => c.Location);
        result.ShouldNotHaveValidationErrorFor(c => c.ParticipantsCount);
        result.ShouldNotHaveValidationErrorFor(c => c.MeetingsCount);
    }
}
