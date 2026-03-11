using Moq;
using AutoMapper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Services.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
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
