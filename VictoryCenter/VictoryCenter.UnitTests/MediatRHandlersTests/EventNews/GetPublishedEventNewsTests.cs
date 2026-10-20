using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
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

    public GetPublishedEventNewsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    public static IEnumerable<object?[]> OffsetAndLimitCases()
    {
        yield return new object?[] { 8, 4, 8, 4 };
        yield return new object?[] { null, null, 0, EventNewsConstants.DefaultLimit };
        yield return new object?[] { -5, 4, 0, 4 };
        yield return new object?[]
        {
            0,
            EventNewsConstants.PublishedTakeMaxValue + 50,
            0,
            EventNewsConstants.PublishedTakeMaxValue
        };
        yield return new object?[]
        {
            0,
            EventNewsConstants.PublishedTakeMinValue - 1,
            0,
            EventNewsConstants.PublishedTakeMinValue
        };
    }

    [Fact]
    public void Mapping_MapsCategoryBaseNameAndLocalizations()
    {
        var language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var entity = new EventNewsEntity
        {
            Id = 1,
            Category =
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
                }
        };
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<EventNewsProfile>();
            config.AddProfile<LocalizationsLanguageProfile>();
        });
        var mapper = configuration.CreateMapper();

        var result = mapper.Map<PublishedEventNewsDto>(entity);

        var category = result.Category;
        Assert.Equal("Base category name", category.Name);
        var localization = Assert.Single(category.Localizations);
        Assert.Equal("Localized category name", localization.Name);
        Assert.Equal("en", localization.Language.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnPublishedEventNewsWithTotalCount()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 7);

        Result<PaginationResult<PublishedEventNewsDto>> result =
            await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Length);
        Assert.Equal(7, result.Value.TotalItemsCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemsInOrderOfPagedIds_NotInOrderReturnedByDatabase()
    {
        var entities = new List<EventNewsEntity>
        {
            new() { Id = 1, Status = Status.Published },
            new() { Id = 2, Status = Status.Published },
            new() { Id = 3, Status = Status.Published },
        };
        SetUpDependencies(entities, totalCount: 3, pagedIds: [3L, 1L, 2L]);

        var result = await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.Equal([3L, 1L, 2L], result.Value.Items.Select(i => i.Id).ToArray());
    }

    [Fact]
    public async Task Handle_WhenLoadedItemIsNotInPagedIds_ShouldPlaceItAtTheEndWithoutThrowing()
    {
        var entities = new List<EventNewsEntity>
        {
            new() { Id = 99, Status = Status.Published },
            new() { Id = 1, Status = Status.Published },
        };
        SetUpDependencies(entities, totalCount: 2, pagedIds: [1L]);

        var result = await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal([1L, 99L], result.Value.Items.Select(i => i.Id).ToArray());
    }

    [Fact]
    public async Task Handle_WhenPagedIdsAreEmpty_ShouldReturnEmptyPageWithoutLoadingItems()
    {
        SetUpDependencies([], totalCount: 5, pagedIds: Array.Empty<long>());

        var result = await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(5, result.Value.TotalItemsCount);
        VerifyPageQueryNeverExecuted();
    }

    [Fact]
    public async Task Handle_WhenNoPublishedItems_ShouldNotRequestIdsOrItems()
    {
        SetUpDependencies([], totalCount: 0);

        var result = await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalItemsCount);
        VerifyIdsQueryNeverExecuted();
        VerifyPageQueryNeverExecuted();
    }

    [Fact]
    public async Task Handle_WhenOffsetIsGreaterThanOrEqualToTotal_ShouldNotRequestIdsOrItems()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 5);

        var result = await CreateHandler().Handle(CreateQuery(offset: 5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(5, result.Value.TotalItemsCount);
        VerifyIdsQueryNeverExecuted();
        VerifyPageQueryNeverExecuted();
    }

    [Theory]
    [MemberData(nameof(OffsetAndLimitCases))]
    public async Task Handle_ShouldPassNormalizedOffsetAndLimitToPagedIdsQuery(
        int? offset,
        int? limit,
        int expectedOffset,
        int expectedLimit)
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 500);

        await CreateHandler().Handle(CreateQuery(offset: offset, limit: limit), CancellationToken.None);

        VerifyIdsQuery(expectedOffset, expectedLimit);
    }

    [Fact]
    public async Task Handle_ShouldNotApplyOrderingToItemsQuery_AndUseReadOnlyOptions()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 2);

        await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        VerifyPageQuery(o =>
            o.OrderByDESC == null &&
            o.ThenByDESC == null &&
            o.AsSplitQuery &&
            o.AsNoTracking);
    }

    [Fact]
    public async Task Handle_ShouldUseSameFilterForCountAndPagedIdsQuery()
    {
        Expression<Func<EventNewsEntity, bool>>? countFilter = null;
        SetUpDependencies(_eventNewsEntities, totalCount: 2);
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(o => countFilter = o.Filter)
            .ReturnsAsync(2);
        var getIdsFilter = CaptureIdsFilter();

        await CreateHandler().Handle(CreateQuery(categoryId: 10), CancellationToken.None);

        Assert.NotNull(countFilter);
        Assert.NotNull(getIdsFilter());
        Assert.Same(countFilter, getIdsFilter());
    }

    [Fact]
    public async Task Handle_ShouldLoadOnlyItemsFromPagedIds()
    {
        Expression<Func<EventNewsEntity, bool>>? itemsFilter = null;
        SetUpDependencies(_eventNewsEntities, totalCount: 5, pagedIds: [2L, 4L]);
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(o => itemsFilter = o.Filter)
            .ReturnsAsync(_eventNewsEntities);

        await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.NotNull(itemsFilter);
        var matched = BuildFilterTestData().Where(itemsFilter.Compile()).Select(e => e.Id).ToList();
        Assert.Equal([2L, 4L], matched);
    }

    [Fact]
    public async Task Handle_WithoutCategory_ShouldFilterOnlyPublishedItems()
    {
        var filter = await CaptureIdsFilterAsync(CreateQuery());

        var matched = BuildFilterTestData().Where(filter.Compile()).Select(e => e.Id).ToList();

        Assert.Equal([1L, 2L, 3L], matched);
    }

    [Fact]
    public async Task Handle_WithCategory_ShouldFilterPublishedItemsOfThatCategory()
    {
        var filter = await CaptureIdsFilterAsync(CreateQuery(categoryId: 10));

        var matched = BuildFilterTestData().Where(filter.Compile()).Select(e => e.Id).ToList();

        Assert.Equal([1L, 3L], matched);
    }

    private Func<Expression<Func<EventNewsEntity, bool>>?> CaptureIdsFilter()
    {
        Expression<Func<EventNewsEntity, bool>>? captured = null;
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetPagedIdsByFilterAsync(
                It.IsAny<Expression<Func<EventNewsEntity, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<EventNewsEntity, bool>>, int, int, CancellationToken>(
                (filter, _, _, _) => captured = filter)
            .ReturnsAsync((IReadOnlyCollection<long>)_eventNewsEntities.Select(e => e.Id).ToList());
        return () => captured;
    }

    private async Task<Expression<Func<EventNewsEntity, bool>>> CaptureIdsFilterAsync(
        GetPublishedEventNewsQuery query)
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 2);
        var getFilter = CaptureIdsFilter();

        await CreateHandler().Handle(query, CancellationToken.None);

        var captured = getFilter();
        Assert.NotNull(captured);
        return captured;
    }

    private void VerifyIdsQuery(int offset, int limit)
    {
        _mockRepositoryWrapper.Verify(
            x => x.EventNewsRepository.GetPagedIdsByFilterAsync(
                It.IsAny<Expression<Func<EventNewsEntity, bool>>>(),
                offset,
                limit,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void VerifyIdsQueryNeverExecuted()
    {
        _mockRepositoryWrapper.Verify(
            x => x.EventNewsRepository.GetPagedIdsByFilterAsync(
                It.IsAny<Expression<Func<EventNewsEntity, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void VerifyPageQuery(Expression<Func<QueryOptions<EventNewsEntity>, bool>> predicate)
    {
        _mockRepositoryWrapper.Verify(
            x => x.EventNewsRepository.GetAllAsync(It.Is(predicate)),
            Times.Once);
    }

    private void VerifyPageQueryNeverExecuted()
    {
        _mockRepositoryWrapper.Verify(
            x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()),
            Times.Never);
    }

    private void SetUpDependencies(
        IEnumerable<EventNewsEntity> items,
        int totalCount,
        IReadOnlyCollection<long>? pagedIds = null)
    {
        var itemsList = items.ToList();

        _mapperMock
            .Setup(x => x.Map<PublishedEventNewsDto[]>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns((IEnumerable<EventNewsEntity> source) =>
                source.Select(x => new PublishedEventNewsDto
                {
                    Id = x.Id,
                    Resource = x.Resource
                }).ToArray());

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(totalCount);

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetPagedIdsByFilterAsync(
                It.IsAny<Expression<Func<EventNewsEntity, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedIds ?? itemsList.Select(x => x.Id).ToList());

        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(itemsList);
    }

    private static GetPublishedEventNewsQuery CreateQuery(
        long? categoryId = null,
        int? offset = null,
        int? limit = null) => new(categoryId, offset, limit);

    private GetPublishedEventNewsHandler CreateHandler() =>
        new(_mapperMock.Object, _mockRepositoryWrapper.Object);

    private static List<EventNewsEntity> BuildFilterTestData() =>
    [
        new() { Id = 1, Status = Status.Published, CategoryId = 10 },
        new() { Id = 2, Status = Status.Published, CategoryId = 20 },
        new() { Id = 3, Status = Status.Published, CategoryId = 10 },
        new() { Id = 4, Status = Status.Draft, CategoryId = 10 },
        new() { Id = 5, Status = Status.Draft, CategoryId = 20 },
    ];
}
