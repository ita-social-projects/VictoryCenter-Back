using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.BLL.Mapping.EventNews;
using VictoryCenter.BLL.Mapping.Localization.Languages;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews;

public class GetPublishedEventNewsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly List<EventNewsEntity> _eventNewsEntities =
    [
        new()
        {
            Id = 1,
            Resource = "NV",
            Status = Status.Published
        },
        new()
        {
            Id = 2,
            Resource = "Канал Дім",
            Status = Status.Published
        },
    ];

    private readonly IEnumerable<PublishedEventNewsDto> _eventNewsDtos =
    [
        new() { Id = 1, Resource = "NV" },
        new() { Id = 2, Resource = "Канал Дім" },
    ];

    public GetPublishedEventNewsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public void Mapping_MapsCategoryBaseNameAndLocalizations()
    {
        var language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var entity = new EventNewsEntity
        {
            Id = 1,
            Categories =
            [
                new EventNewsCategory
                {
                    Id = 10,
                    Name = "Base category name",
                    Localizations =
                    [
                        new EventNewsCategoryLocalization
                        {
                            EntityId = 10,
                            LanguageId = language.Id,
                            Language = language,
                            Name = "Localized category name"
                        },
                    ]
                },
            ]
        };
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<EventNewsProfile>();
            config.AddProfile<LocalizationsLanguageProfile>();
        });
        var mapper = configuration.CreateMapper();

        var result = mapper.Map<PublishedEventNewsDto>(entity);

        var category = Assert.Single(result.Categories);
        Assert.Equal("Base category name", category.Name);
        var localization = Assert.Single(category.Localizations);
        Assert.Equal("Localized category name", localization.Name);
        Assert.Equal("en", localization.Language.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnPublishedEventNews()
    {
        SetUpDependencies(_eventNewsEntities);

        var handler = new GetPublishedEventNewsHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedEventNewsDto>> result =
            await handler.Handle(new GetPublishedEventNewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_WhenNoPublishedItems_ShouldReturnEmptyList()
    {
        SetUpDependencies([]);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<PublishedEventNewsDto>>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns([]);

        var handler = new GetPublishedEventNewsHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedEventNewsDto>> result =
            await handler.Handle(new GetPublishedEventNewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WhenTakeIsProvided_ShouldPassLimitToQueryOptions()
    {
        const int take = 4;
        SetUpDependencies(_eventNewsEntities);

        var handler = new GetPublishedEventNewsHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedEventNewsDto>> result =
            await handler.Handle(new GetPublishedEventNewsQuery(take), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.EventNewsRepository.GetAllAsync(
                It.Is<QueryOptions<EventNewsEntity>>(o =>
                    o.Limit == take &&
                    o.OrderByDESC != null &&
                    o.ThenByDESC != null &&
                    o.AsSplitQuery)),
            Times.Once);
    }

    private void SetUpDependencies(IEnumerable<EventNewsEntity> items)
    {
        _mapperMock
            .Setup(x => x.Map<IEnumerable<PublishedEventNewsDto>>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns(_eventNewsDtos);

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(items);
    }
}
