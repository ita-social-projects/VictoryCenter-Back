using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.HippotherapyPrograms;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests;

public class ProgramSectionContentLocalizationTrackerTests
{
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();
    private readonly Mock<ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization>> _mockContentLocalizationService = new();
    private readonly Mock<TimeProvider> _mockTimeProvider = new();
    private readonly ProgramSectionContentLocalizationTracker _tracker;

    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public ProgramSectionContentLocalizationTrackerTests()
    {
        _mockTimeProvider.Setup(t => t.GetUtcNow()).Returns(Now);

        _tracker = new ProgramSectionContentLocalizationTracker(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _mockContentLocalizationService.Object,
            _mockTimeProvider.Object);
    }

    [Fact]
    public async Task TrackAsync_NewContent_CreatesLocalizationWithCurrentTimestamp()
    {
        // Arrange
        var contentDtos = new List<UpdateHippotherapyProgramSectionContentLocalizationDto>
        {
            new() { EntityId = 200, Title = "New title" }
        };

        SetupMapper(contentDtos.Count);
        SetupExistingLocalizations([]);

        List<ProgramSectionContentLocalization>? capturedCreateBatch = null;
        _mockContentLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<ProgramSectionContentLocalization>>(), false))
            .Callback<IEnumerable<ProgramSectionContentLocalization>, bool>((locs, _) => capturedCreateBatch = locs.ToList())
            .Returns(Task.CompletedTask);

        // Act
        await _tracker.TrackAsync(contentDtos, languageId: 2);

        // Assert
        Assert.NotNull(capturedCreateBatch);
        var created = Assert.Single(capturedCreateBatch!);
        Assert.Equal(200, created.EntityId);
        Assert.Equal(Now, created.CreatedAt);
        Assert.Equal(TranslationStatus.Relevant, created.TranslationStatus);
        _mockContentLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<ProgramSectionContentLocalization>>(), true),
            Times.Never);
    }

    [Fact]
    public async Task TrackAsync_ExistingContent_UpdatesAndPreservesOriginalCreatedAt()
    {
        // Arrange
        var originalCreatedAt = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var contentDtos = new List<UpdateHippotherapyProgramSectionContentLocalizationDto>
        {
            new() { EntityId = 200, Title = "Updated title" }
        };

        SetupMapper(contentDtos.Count);
        SetupExistingLocalizations(
        [
            new() { EntityId = 200, LanguageId = 2, CreatedAt = originalCreatedAt }
        ]);

        List<ProgramSectionContentLocalization>? capturedUpdateBatch = null;
        _mockContentLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<ProgramSectionContentLocalization>>(), true))
            .Callback<IEnumerable<ProgramSectionContentLocalization>, bool>((locs, _) => capturedUpdateBatch = locs.ToList())
            .Returns(Task.CompletedTask);

        // Act
        await _tracker.TrackAsync(contentDtos, languageId: 2);

        // Assert
        Assert.NotNull(capturedUpdateBatch);
        var updated = Assert.Single(capturedUpdateBatch!);
        Assert.Equal(originalCreatedAt, updated.CreatedAt);
        _mockContentLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<ProgramSectionContentLocalization>>(), false),
            Times.Never);
    }

    [Fact]
    public async Task TrackAsync_MixedBatch_CreatesNewContentAndUpdatesExistingContentSeparately()
    {
        // Arrange
        var originalCreatedAt = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var contentDtos = new List<UpdateHippotherapyProgramSectionContentLocalizationDto>
        {
            new() { EntityId = 100, Title = "Existing section title" },
            new() { EntityId = 200, Title = "New section title" }
        };

        SetupMapper(contentDtos.Count);
        SetupExistingLocalizations(
        [
            new() { EntityId = 100, LanguageId = 2, CreatedAt = originalCreatedAt }
        ]);

        _mockContentLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<ProgramSectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tracker.TrackAsync(contentDtos, languageId: 2);

        // Assert
        _mockContentLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<ProgramSectionContentLocalization>>(l => l.Single().EntityId == 100),
                true),
            Times.Once);
        _mockContentLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<ProgramSectionContentLocalization>>(l => l.Single().EntityId == 200),
                false),
            Times.Once);
    }

    private void SetupMapper(int count)
    {
        _mockMapper
            .Setup(m => m.Map<List<ProgramSectionContentLocalization>>(It.IsAny<List<UpdateHippotherapyProgramSectionContentLocalizationDto>>()))
            .Returns(Enumerable.Range(0, count).Select(_ => new ProgramSectionContentLocalization()).ToList());
    }

    private void SetupExistingLocalizations(List<ProgramSectionContentLocalization> existing)
    {
        _mockRepositoryWrapper
            .Setup(r => r.ProgramSectionContentLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<ProgramSectionContentLocalization>>()))
            .ReturnsAsync(existing);
    }
}
