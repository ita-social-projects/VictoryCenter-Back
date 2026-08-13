using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests;

public class ProgramSectionContentServiceTests
{
    private readonly ProgramSectionContentService _service;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public ProgramSectionContentServiceTests()
    {
        _service = new ProgramSectionContentService(_repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldReturnContentTypeDictionary_WhenProgramExistsWithSectionsAndContents()
    {
        // Arrange
        const long programId = 1;

        // Create content objects (concrete types to represent abstract ProgramSectionContent)
        var content1 = CreateTestContent(1, ContentType.Title);
        var content2 = CreateTestContent(2, ContentType.Description);
        var content3 = CreateTestContent(3, ContentType.Image);

        var section1 = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = programId,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = new List<ProgramSectionContent> { content1, content2 }
        };

        var section2 = new HippotherapyProgramSection
        {
            Id = 11,
            ProgramId = programId,
            Template = ProgramSectionTemplate.SingleImageBottom,
            Order = 2,
            Contents = new List<ProgramSectionContent> { content3 }
        };

        var program = new HippotherapyProgram
        {
            Id = programId,
            Name = "Test Program",
            Slug = "test-program",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection> { section1, section2 }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program);

        // Act
        var result = await _service.GetContentTypesByProgramIdAsync(programId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(ContentType.Title, result[1]);
        Assert.Equal(ContentType.Description, result[2]);
        Assert.Equal(ContentType.Image, result[3]);
        _repositoryWrapperMock.Verify(
            x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldReturnEmptyDictionary_WhenProgramHasNoSections()
    {
        // Arrange
        const long programId = 1;

        var program = new HippotherapyProgram
        {
            Id = programId,
            Name = "Test Program",
            Slug = "test-program",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection>()
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program);

        // Act
        var result = await _service.GetContentTypesByProgramIdAsync(programId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldReturnEmptyDictionary_WhenSectionsHaveNoContents()
    {
        // Arrange
        const long programId = 1;

        var section1 = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = programId,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = new List<ProgramSectionContent>()
        };

        var section2 = new HippotherapyProgramSection
        {
            Id = 11,
            ProgramId = programId,
            Template = ProgramSectionTemplate.SingleImageBottom,
            Order = 2,
            Contents = new List<ProgramSectionContent>()
        };

        var program = new HippotherapyProgram
        {
            Id = programId,
            Name = "Test Program",
            Slug = "test-program",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection> { section1, section2 }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program);

        // Act
        var result = await _service.GetContentTypesByProgramIdAsync(programId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldThrowKeyNotFoundException_WhenProgramDoesNotExist()
    {
        // Arrange
        const long programId = 999;

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync((HippotherapyProgram?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.GetContentTypesByProgramIdAsync(programId));

        Assert.Equal(ErrorMessagesConstants.NotFound(programId, typeof(HippotherapyProgram)), ex.Message);
        _repositoryWrapperMock.Verify(
            x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldFlattenAllSectionsContents_IntoSingleDictionary()
    {
        // Arrange
        const long programId = 1;

        var content1 = CreateTestContent(1, ContentType.Title);
        var content2 = CreateTestContent(2, ContentType.Description);
        var content3 = CreateTestContent(3, ContentType.Image);
        var content4 = CreateTestContent(4, ContentType.Card);

        var section1 = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = programId,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = new List<ProgramSectionContent> { content1, content2 }
        };

        var section2 = new HippotherapyProgramSection
        {
            Id = 11,
            ProgramId = programId,
            Template = ProgramSectionTemplate.SingleImageBottom,
            Order = 2,
            Contents = new List<ProgramSectionContent> { content3, content4 }
        };

        var program = new HippotherapyProgram
        {
            Id = programId,
            Name = "Test Program",
            Slug = "test-program",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection> { section1, section2 }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program);

        // Act
        var result = await _service.GetContentTypesByProgramIdAsync(programId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.True(result.ContainsKey(1));
        Assert.True(result.ContainsKey(2));
        Assert.True(result.ContainsKey(3));
        Assert.True(result.ContainsKey(4));
        Assert.Equal(ContentType.Title, result[1]);
        Assert.Equal(ContentType.Description, result[2]);
        Assert.Equal(ContentType.Image, result[3]);
        Assert.Equal(ContentType.Card, result[4]);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldUseAsNoTracking_WhenQueryingProgram()
    {
        // Arrange
        const long programId = 1;

        var content = CreateTestContent(1, ContentType.Title);
        var section = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = programId,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = new List<ProgramSectionContent> { content }
        };

        var program = new HippotherapyProgram
        {
            Id = programId,
            Name = "Test Program",
            Slug = "test-program",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection> { section }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program)
            .Callback((QueryOptions<HippotherapyProgram> options) =>
            {
                // Verify AsNoTracking is true
                Assert.True(options.AsNoTracking);
            });

        // Act
        var result = await _service.GetContentTypesByProgramIdAsync(programId);

        // Assert
        Assert.NotNull(result);
        _repositoryWrapperMock.Verify(
            x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetContentTypesByProgramIdAsync_ShouldIncludeSectionsAndContents_InQuery()
    {
        // Arrange
        const long programId = 1;

        var content = CreateTestContent(1, ContentType.Title);
        var section = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = programId,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = new List<ProgramSectionContent> { content }
        };

        var program = new HippotherapyProgram
        {
            Id = programId,
            Name = "Test Program",
            Slug = "test-program",
            Status = Status.Published,
            Sections = new List<HippotherapyProgramSection> { section }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program);

        // Act
        var result = await _service.GetContentTypesByProgramIdAsync(programId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        _repositoryWrapperMock.Verify(
            x => x.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProgramSectionsAsync_ShouldReturnNotFoundExceptionAsync()
    {
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()))
            .ReturnsAsync((HippotherapyProgramLocalization)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
             await _service.GetProgramSectionsAsync(1, 1));

        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgram)), ex.Message);
    }

    [Fact]
    public async Task GetProgramSectionsAsync_ShouldReturnSectionsSuccessFully()
    {
        var content = new TestProgramSectionContent
        {
            Id = 1,
            ContentType = ContentType.Title,
            Order = 1,
            SectionId = 1,
            Localizations = new List<ProgramSectionContentLocalization>
            {
                new()
                {
                    EntityId = 1,
                    LanguageId = 1,
                    Title = "Test Title",
                    TranslationStatus = TranslationStatus.Relevant,
                    Language = new LocalizationLanguage
                    {
                        Id = 1,
                        Code = "en",
                        Name = "English"
                    },
                    CreatedAt = DateTimeOffset.UtcNow
                }
            }
        };

        var section = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = 1,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = new List<ProgramSectionContent> { content }
        };

        var programLocalization = new HippotherapyProgramLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            Language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" },
            Entity = new HippotherapyProgram
            {
                Id = 1,
                Name = "Test Program",
                Slug = "test-program",
                Status = Status.Published,
                Sections = new List<HippotherapyProgramSection> { section }
            }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()))
            .ReturnsAsync(programLocalization);

        _mapperMock
            .Setup(x => x.Map<HippotherapyProgramSectionContentLocalizationDto>(It.IsAny<ProgramSectionContentLocalization>()))
            .Returns((ProgramSectionContentLocalization src) => new HippotherapyProgramSectionContentLocalizationDto
            {
                EntityId = src.EntityId,
                Title = src.Title,
                Description = src.Description,
                Author = src.Author,
                Question = src.Question,
                Answer = src.Answer,
                TranslationStatus = src.TranslationStatus
            });

        var result = await _service.GetProgramSectionsAsync(1, 1);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        _repositoryWrapperMock.Verify(
            x => x.HippotherapyProgramsLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProgramSectionsAsync_ShouldReturnEmptyOutdatedPlaceholder_WhenContentHasNoLocalizationForLanguage()
    {
        var translatedContent = new TestProgramSectionContent
        {
            Id = 1,
            ContentType = ContentType.Title,
            Order = 0,
            SectionId = 10,
            Localizations =
            [
                new()
                {
                    EntityId = 1,
                    LanguageId = 1,
                    Title = "Translated title",
                    TranslationStatus = TranslationStatus.Relevant
                }

            ]
        };

        var newlyAddedContent = new TestProgramSectionContent
        {
            Id = 2,
            ContentType = ContentType.Description,
            Order = 1,
            SectionId = 10,
            Localizations = []
        };

        var section = new HippotherapyProgramSection
        {
            Id = 10,
            ProgramId = 1,
            Template = ProgramSectionTemplate.TextOnly,
            Order = 1,
            Contents = [translatedContent, newlyAddedContent]
        };

        var programLocalization = new HippotherapyProgramLocalization
        {
            EntityId = 1,
            LanguageId = 1,
            Language = new LocalizationLanguage { Id = 1, Code = "en", Name = "English" },
            Entity = new HippotherapyProgram
            {
                Id = 1,
                Name = "Test Program",
                Slug = "test-program",
                Status = Status.Published,
                Sections = [section]
            }
        };

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramsLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()))
            .ReturnsAsync(programLocalization);

        _mapperMock
            .Setup(x => x.Map<HippotherapyProgramSectionContentLocalizationDto>(It.IsAny<ProgramSectionContentLocalization>()))
            .Returns((ProgramSectionContentLocalization src) => new HippotherapyProgramSectionContentLocalizationDto
            {
                EntityId = src.EntityId,
                Title = src.Title,
                TranslationStatus = src.TranslationStatus
            });

        _mapperMock
            .Setup(x => x.Map<LocalizationInfoDto>(It.IsAny<LocalizationLanguage>()))
            .Returns((LocalizationLanguage src) => new LocalizationInfoDto { Id = src.Id, Code = src.Code });

        var result = await _service.GetProgramSectionsAsync(1, 1);

        var contents = Assert.Single(result).Contents;
        Assert.Equal(2, contents.Count);

        var translatedDto = Assert.Single(contents, c => c.EntityId == 1);
        Assert.Equal("Translated title", translatedDto.Title);
        Assert.Equal(TranslationStatus.Relevant, translatedDto.TranslationStatus);

        var placeholderDto = Assert.Single(contents, c => c.EntityId == 2);
        Assert.Equal(TranslationStatus.Outdated, placeholderDto.TranslationStatus);
        Assert.Null(placeholderDto.Title);
        Assert.Null(placeholderDto.Description);
        Assert.Null(placeholderDto.Author);
        Assert.Null(placeholderDto.Question);
        Assert.Null(placeholderDto.Answer);
        Assert.Equal(1, placeholderDto.LocalizationInfoDto.Id);
        Assert.Equal("en", placeholderDto.LocalizationInfoDto.Code);
    }

    private static ProgramSectionContent CreateTestContent(long id, ContentType contentType)
    {
        return new TestProgramSectionContent
        {
            Id = id,
            ContentType = contentType,
            Order = 1,
            SectionId = 1
        };
    }

    private class TestProgramSectionContent : ProgramSectionContent
    {
    }
}
