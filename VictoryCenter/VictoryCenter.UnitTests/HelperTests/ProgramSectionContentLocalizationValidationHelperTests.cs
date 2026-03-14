using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.UnitTests.HelperTests;

public class ProgramSectionContentLocalizationValidationHelperTests
{
    [Fact]
    public void ValidateSections_NoSectionsAndNoProgramSections_DoesNotThrow()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>();
        var contentTypesById = new Dictionary<long, ContentType>();
        var program = CreateProgram();

        ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
            sections,
            contentTypesById,
            content => content.EntityId,
            program);
    }

    [Fact]
    public void ValidateSections_MissingSectionInRequest_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>();
        var contentTypesById = new Dictionary<long, ContentType>();
        var program = CreateProgram(CreateSection(1));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains("Missing sections in localization request", ex.Message);
    }

    [Fact]
    public void ValidateSections_UnknownSectionInRequest_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 99,
                Contents = []
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>();
        var program = CreateProgram(CreateSection(1));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains("Missing sections in localization request", ex.Message);
    }

    [Fact]
    public void ValidateSections_DuplicateContentIds_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 10, Title = "A" },
                    new() { EntityId = 10, Title = "B" }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title }
        };

        var program = CreateProgram(CreateSection(1, (10, ContentType.Title)));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains("duplicate content ids", ex.Message);
    }

    [Fact]
    public void ValidateSections_MissingRequiredContentIds_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 10, Title = "A" }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title },
            { 11, ContentType.Description }
        };

        var program = CreateProgram(CreateSection(1, (10, ContentType.Title), (11, ContentType.Description)));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains("missing required content ids", ex.Message);
    }

    [Fact]
    public void ValidateSections_ContentCountMismatch_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 10, Title = "A" },
                    new() { EntityId = 12, Description = "img payload" }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title },
            { 12, ContentType.Image }
        };

        var program = CreateProgram(CreateSection(1, (10, ContentType.Title), (12, ContentType.Image)));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains("expected 1 required contents", ex.Message);
    }

    [Fact]
    public void ValidateSections_InvalidContentId_ThrowsNotFoundValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 10, Title = "A" }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>();
        var program = CreateProgram(CreateSection(1, (10, ContentType.Title)));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains(
            ErrorMessagesConstants.NotFound(10, typeof(ProgramSectionContent)).Split(' ')[0],
            ex.Message);
    }

    [Fact]
    public void ValidateSections_TitleType_WithoutTitle_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 10, Title = "" }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title }
        };

        var program = CreateProgram(CreateSection(1, (10, ContentType.Title)));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired("Title"), ex.Message);
    }

    [Fact]
    public void ValidateSections_DescriptionType_WithForbiddenTitle_ThrowsValidationException()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 11, Description = "D", Title = "Forbidden" }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 11, ContentType.Description }
        };

        var program = CreateProgram(CreateSection(1, (11, ContentType.Description)));

        var ex = Assert.Throws<ValidationException>(() =>
            ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
                sections,
                contentTypesById,
                content => content.EntityId,
                program));

        Assert.Contains(
            ErrorMessagesConstants.PropertyNotAllowedForContentType("Title", ContentType.Description),
            ex.Message);
    }

    [Fact]
    public void ValidateSections_AllRequiredTypesValid_DoesNotThrow()
    {
        var sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                Contents =
                [
                    new() { EntityId = 10, Title = "Title" },
                    new() { EntityId = 11, Description = "Description" },
                    new() { EntityId = 12, Author = "Author" },
                    new() { EntityId = 13, Question = "Question?", Answer = "Answer." }
                ]
            }
        };

        var contentTypesById = new Dictionary<long, ContentType>
        {
            { 10, ContentType.Title },
            { 11, ContentType.Description },
            { 12, ContentType.Author },
            { 13, ContentType.FaqQuestion },
            { 14, ContentType.Image }
        };

        var program = CreateProgram(
            CreateSection(
                1,
                (10, ContentType.Title),
                (11, ContentType.Description),
                (12, ContentType.Author),
                (13, ContentType.FaqQuestion),
                (14, ContentType.Image)));

        ProgramSectionContentLocalizationValidationHelper.ValidateSections<
            CreateHippotherapyProgramSectionLocalizationDto,
            CreateHippotherapyProgramSectionContentLocalizationDto
        >(
            sections,
            contentTypesById,
            content => content.EntityId,
            program);
    }

    private static HippotherapyProgramEntity CreateProgram(params HippotherapyProgramSection[] sections)
    {
        return new HippotherapyProgramEntity
        {
            Sections = sections.ToList()
        };
    }

    private static HippotherapyProgramSection CreateSection(
        long sectionId,
        params (long contentId, ContentType contentType)[] contents)
    {
        return new HippotherapyProgramSection
        {
            Id = sectionId,
            Contents = contents
                .Select(content => new TestProgramSectionContent
                {
                    Id = content.contentId,
                    ContentType = content.contentType,
                    SectionId = sectionId
                })
                .Cast<ProgramSectionContent>()
                .ToList()
        };
    }

    private sealed class TestProgramSectionContent : ProgramSectionContent
    {
    }
}
