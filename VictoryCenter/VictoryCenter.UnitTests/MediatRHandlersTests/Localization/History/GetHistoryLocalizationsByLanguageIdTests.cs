using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.Queries.Admin.Localization.History.GetByLanguageId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.History;

public class GetHistoryLocalizationsByLanguageIdTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetHistoryLocalizationsByLanguageIdHandler _handler;

    private readonly long _testLanguageId = 1;

    public GetHistoryLocalizationsByLanguageIdTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetHistoryLocalizationsByLanguageIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_Successfully()
    {
        var sections = new List<HistorySection>
        {
            new()
            {
                Id = 1,
                Contents = new List<HistorySectionContent>
                {
                    new TitleHistoryContent
                    {
                        Id = 1,
                        Localizations = new List<HistorySectionContentLocalization>
                        {
                            new() { LanguageId = _testLanguageId, EntityId = 1, Title = "Title UK" }
                        }
                    }
                }
            },
            new()
            {
                Id = 2,
                Contents = new List<HistorySectionContent>
                {
                    new DescriptionHistoryContent
                    {
                        Id = 2,
                        Localizations = new List<HistorySectionContentLocalization>
                        {
                            new() { LanguageId = _testLanguageId, EntityId = 2, Description = "Desc UK" }
                        }
                    }
                }
            }
        };

        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(sections);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<IEnumerable<HistorySectionContentLocalization>>()))
            .Returns(new List<HistorySectionContentLocalizationDto>
            {
                new() { LanguageId = _testLanguageId }
            });

        var command = new GetHistoryLocalizationsByLanguageIdQuery(_testLanguageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(1, result.Value[0].EntityId);
        Assert.Equal(2, result.Value[1].EntityId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoLocalizationsFound()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection>());

        var command = new GetHistoryLocalizationsByLanguageIdQuery(_testLanguageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSectionsHaveNoContents()
    {
        var sections = new List<HistorySection>
        {
            new() { Id = 1, Contents = new List<HistorySectionContent>() }
        };

        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(sections);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<IEnumerable<HistorySectionContentLocalization>>()))
            .Returns(new List<HistorySectionContentLocalizationDto>());

        var command = new GetHistoryLocalizationsByLanguageIdQuery(_testLanguageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Empty(result.Value[0].Contents);
    }
}
