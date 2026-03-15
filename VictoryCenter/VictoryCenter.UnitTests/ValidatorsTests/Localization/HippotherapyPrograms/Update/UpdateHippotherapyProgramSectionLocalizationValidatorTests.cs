using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramSectionLocalizationValidatorTests
{
    private readonly UpdateHippotherapyProgramSectionLocalizationValidator _validator;

    public UpdateHippotherapyProgramSectionLocalizationValidatorTests()
    {
        var baseSectionContentValidator = new BaseProgramSectionContentLocalizationValidator();
        var updateSectionContentValidator = new UpdateHippotherapyProgramSectionContentLocalizationValidator(baseSectionContentValidator);

        _validator = new UpdateHippotherapyProgramSectionLocalizationValidator(updateSectionContentValidator);
    }

    [Fact]
    public void Validate_WhenEntityIdIsNotPositive_ShouldHaveValidationError()
    {
        var dto = new UpdateHippotherapyProgramSectionLocalizationDto
        {
            EntityId = 0,
            Contents = []
        };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "EntityId must be greater than 0.");
    }

    [Fact]
    public void Validate_WhenContentHasInvalidEntityId_ShouldHaveValidationError()
    {
        var dto = new UpdateHippotherapyProgramSectionLocalizationDto
        {
            EntityId = 1,
            Contents =
            [
                new UpdateHippotherapyProgramSectionContentLocalizationDto
                {
                    EntityId = 0,
                    Title = "Valid title"
                }

            ]
        };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "EntityId must be greater than 0.");
    }

    [Fact]
    public void Validate_WhenContentsIsNull_ShouldNotHaveContentValidationErrors()
    {
        var dto = new UpdateHippotherapyProgramSectionLocalizationDto
        {
            EntityId = 1,
            Contents = null
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WhenDtoIsValid_ShouldBeValid()
    {
        var dto = new UpdateHippotherapyProgramSectionLocalizationDto
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
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
