using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.HelperTests;

public class ProgramSectionContentLocalizationValidationHelperTests
{
    [Fact]
    public void ValidateSections_NoSections_DoesNotThrow()
    {
        ProgramSectionContentLocalizationValidationHelper.ValidateSections(
            new List<CreateHippotherapyProgramSectionLocalizationDto>(),
            new Dictionary<long, ContentType>());
    }

    [Fact]
    public void ValidateSections_CountMismatch_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                {
                    new() { EntityId = 1, Title = "A" }
                }
            }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections(sections, new Dictionary<long, ContentType>()));

        Assert.Contains("Number of section contents", ex.Message);
    }

    [Fact]
    public void ValidateSections_InvalidContentId_AddsNotFoundFailure()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                {
                    new() { EntityId = 5, Title = "some" }
                }
            }
        };

        var dict = new Dictionary<long, ContentType>
        {
            { 1, ContentType.Title }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections(sections, dict));

        Assert.Contains(ErrorMessagesConstants.NotFound(5, typeof(object)).Split(' ')[0], ex.Message);
    }

    [Fact]
    public void ValidateSections_TitleRequired_WhenMissing_ShouldThrow()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                {
                    new() { EntityId = 1, Title = "" }
                }
            }
        };

        var dict = new Dictionary<long, ContentType>
        {
            { 1, ContentType.Title }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections(sections, dict));

        Assert.Contains("Title is required", ex.Message);
    }

    [Fact]
    public void ValidateSections_ForbiddenField_ThrowsForbidMessage()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                {
                    new() { EntityId = 1, Title = "Good", Description = "oops" }
                }
            }
        };

        var dict = new Dictionary<long, ContentType>
        {
            { 1, ContentType.Title }
        };

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections(sections, dict));

        Assert.Contains("Description is not allowed for content type Title", ex.Message);
    }
}
