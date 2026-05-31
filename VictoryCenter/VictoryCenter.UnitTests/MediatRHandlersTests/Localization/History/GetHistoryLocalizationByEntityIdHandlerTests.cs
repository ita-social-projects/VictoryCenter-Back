using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.Queries.Admin.Localization.History.GetByEntityId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.History;

public class GetHistoryLocalizationByEntityIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetHistoryLocalizationByEntityIdHandler _handler;

    private readonly long _testEntityId = 1;

    public GetHistoryLocalizationByEntityIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetHistoryLocalizationByEntityIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnGroupedLocalizations_Successfully()
    {
        var section = new HistorySection
        {
            Id = _testEntityId,
            Contents = new List<HistorySectionContent>
            {
                new TitleHistoryContent
                {
                    Id = 1,
                    Localizations = new List<HistorySectionContentLocalization>
                    {
                        new() { LanguageId = 1, EntityId = 1, Title = "Title UK" },
                        new() { LanguageId = 2, EntityId = 1, Title = "Title EN" }
                    }
                },
                new DescriptionHistoryContent
                {
                    Id = 2,
                    Localizations = new List<HistorySectionContentLocalization>
                    {
                        new() { LanguageId = 1, EntityId = 2, Description = "Desc UK" },
                        new() { LanguageId = 2, EntityId = 2, Description = "Desc EN" }
                    }
                }
            }
        };

        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<IEnumerable<HistorySectionContentLocalization>>()))
            .Returns(new List<HistorySectionContentLocalizationDto>());

        var command = new GetHistoryLocalizationByEntityIdQuery(_testEntityId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSectionNotFound()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync((HistorySection)null!);

        var command = new GetHistoryLocalizationByEntityIdQuery(_testEntityId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(_testEntityId, typeof(HistorySection)), result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSectionHasNoContents()
    {
        var section = new HistorySection
        {
            Id = _testEntityId,
            Contents = new List<HistorySectionContent>()
        };

        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        var command = new GetHistoryLocalizationByEntityIdQuery(_testEntityId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenContentsHaveNoLocalizations()
    {
        var section = new HistorySection
        {
            Id = _testEntityId,
            Contents = new List<HistorySectionContent>
            {
                new TitleHistoryContent
                {
                    Id = 1,
                    Localizations = new List<HistorySectionContentLocalization>()
                }
            }
        };

        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        var command = new GetHistoryLocalizationByEntityIdQuery(_testEntityId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}
