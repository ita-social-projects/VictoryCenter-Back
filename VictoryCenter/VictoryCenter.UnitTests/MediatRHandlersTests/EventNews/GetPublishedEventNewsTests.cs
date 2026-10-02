using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsCategoryLink = VictoryCenter.DAL.Entities.EventNewsEventNewsCategories;
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

    public GetPublishedEventNewsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
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
            .Setup(x => x.Map<List<PublishedEventNewsDto>>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns(new List<PublishedEventNewsDto>());

        var handler = new GetPublishedEventNewsHandler(_mapperMock.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedEventNewsDto>> result =
            await handler.Handle(new GetPublishedEventNewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WhenTakeIsProvided_ShouldLimitResult()
    {
        var items = new List<EventNewsEntity>
        {
            new() { Id = 1, Resource = "NV", Status = Status.Published },
            new() { Id = 2, Resource = "Канал Дім", Status = Status.Published },
        };

        _mapperMock
    .Setup(x => x.Map<List<PublishedEventNewsDto>>(
        It.IsAny<IEnumerable<EventNewsEntity>>()))
    .Returns((IEnumerable<EventNewsEntity> source) =>
        source.Select(x => new PublishedEventNewsDto
        {
            Id = x.Id,
            Resource = x.Resource
        }).ToList());

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(items);

        _mockRepositoryWrapper
    .Setup(x => x.EventNewsEventNewsCategoriesRepository.GetAllAsync(
        It.IsAny<QueryOptions<EventNewsCategoryLink>>()))
    .ReturnsAsync(new List<EventNewsCategoryLink>());

        var handler = new GetPublishedEventNewsHandler(
            _mapperMock.Object,
            _mockRepositoryWrapper.Object);

        var result = await handler.Handle(
            new GetPublishedEventNewsQuery(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(1, result.Value[0].Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEventNewsSortedByPublishedAt()
    {
        var items = new List<EventNewsEntity>
        {
            new()
            {
                Id = 1,
                Resource = "NV",
                Status = Status.Published,
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 2,
                Resource = "Канал Дім",
                Status = Status.Published,
                PublishedAt = DateTimeOffset.UtcNow
            }
        };

        _mapperMock
            .Setup(x => x.Map<List<PublishedEventNewsDto>>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns((IEnumerable<EventNewsEntity> source) =>
                source.Select(x => new PublishedEventNewsDto
                {
                    Id = x.Id,
                    Resource = x.Resource
                }).ToList());

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(items);

        var handler = new GetPublishedEventNewsHandler(
            _mapperMock.Object,
            _mockRepositoryWrapper.Object);

        var result = await handler.Handle(
            new GetPublishedEventNewsQuery(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal([2, 1], result.Value.Select(item => item.Id));
    }

    private void SetUpDependencies(IEnumerable<EventNewsEntity> items)
    {
        _mapperMock
            .Setup(x => x.Map<List<PublishedEventNewsDto>>(
                It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns((IEnumerable<EventNewsEntity> source) =>
                source.Select(x => new PublishedEventNewsDto
                {
                    Id = x.Id,
                    Resource = x.Resource
                }).ToList());

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(items);

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsEventNewsCategoriesRepository.GetAllAsync(
                It.IsAny<QueryOptions<EventNewsCategoryLink>>()))
            .ReturnsAsync(new List<EventNewsCategoryLink>());
    }
}
