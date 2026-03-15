using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramSectionContentLocalizationValidatorTests
{
    private readonly UpdateHippotherapyProgramSectionContentLocalizationValidator _validator;

    public UpdateHippotherapyProgramSectionContentLocalizationValidatorTests()
    {
        var baseValidator = new BaseProgramSectionContentLocalizationValidator();
        _validator = new UpdateHippotherapyProgramSectionContentLocalizationValidator(baseValidator);
    }

    [Fact]
    public void Validate_WhenEntityIdIsNotPositive_ShouldHaveValidationError()
    {
        var dto = new UpdateHippotherapyProgramSectionContentLocalizationDto
        {
            EntityId = 0,
            Title = "Valid title"
        };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "EntityId must be greater than 0.");
    }

    [Fact]
    public void Validate_WhenTitleIsTooShort_ShouldHaveValidationError()
    {
        var dto = new UpdateHippotherapyProgramSectionContentLocalizationDto
        {
            EntityId = 1,
            Title = "abcd"
        };

        var result = _validator.Validate(dto);

        Assert.Contains(
            result.Errors,
            x => x.ErrorMessage == ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Title),
                ProgramSectionContentLocalizationConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_WhenDtoIsValid_ShouldBeValid()
    {
        var dto = new UpdateHippotherapyProgramSectionContentLocalizationDto
        {
            EntityId = 1,
            Title = "Valid title"
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
