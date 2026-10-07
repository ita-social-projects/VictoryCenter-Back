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
    public async Task Handle_ShouldReturnPublishedEventNewsWithTotalCount()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 7);

        Result<PaginationResult<PublishedEventNewsDto>> result =
            await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count());
        Assert.Equal(7, result.Value.TotalItemsCount);
    }

    [Fact]
    public async Task Handle_WhenNoPublishedItems_ShouldReturnEmptyPageWithoutLoadingItems()
    {
        SetUpDependencies([], totalCount: 0);

        var result = await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalItemsCount);
        VerifyPageQueryNeverExecuted();
    }

    [Fact]
    public async Task Handle_WhenOffsetIsGreaterThanOrEqualToTotal_ShouldReturnEmptyPageWithTotalCount()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 5);

        var result = await CreateHandler().Handle(CreateQuery(offset: 5), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(5, result.Value.TotalItemsCount);
        VerifyPageQueryNeverExecuted();
    }

    [Fact]
    public async Task Handle_WhenOffsetAndLimitProvided_ShouldPassThemToQueryOptions()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 50);

        var result = await CreateHandler().Handle(CreateQuery(offset: 8, limit: 4), CancellationToken.None);

        Assert.True(result.IsSuccess);
        VerifyPageQuery(o => o.Offset == 8 && o.Limit == 4);
    }

    [Fact]
    public async Task Handle_WhenOffsetAndLimitAreNull_ShouldUseDefaults()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 50);

        await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        VerifyPageQuery(o => o.Offset == 0 && o.Limit == EventNewsConstants.DefaultLimit);
    }

    [Fact]
    public async Task Handle_WhenOffsetIsNegative_ShouldTreatItAsZero()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 50);

        await CreateHandler().Handle(CreateQuery(offset: -5, limit: 4), CancellationToken.None);

        VerifyPageQuery(o => o.Offset == 0);
    }

    [Fact]
    public async Task Handle_WhenLimitExceedsMaximum_ShouldClampToMaximum()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 500);

        await CreateHandler().Handle(
            CreateQuery(limit: EventNewsConstants.PublishedTakeMaxValue + 50),
            CancellationToken.None);

        VerifyPageQuery(o => o.Limit == EventNewsConstants.PublishedTakeMaxValue);
    }

    [Fact]
    public async Task Handle_WhenLimitIsBelowMinimum_ShouldClampToMinimum()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 500);

        await CreateHandler().Handle(
            CreateQuery(limit: EventNewsConstants.PublishedTakeMinValue - 1),
            CancellationToken.None);

        VerifyPageQuery(o => o.Limit == EventNewsConstants.PublishedTakeMinValue);
    }

    [Fact]
    public async Task Handle_ShouldUseStableOrderingAndReadOnlyOptions()
    {
        SetUpDependencies(_eventNewsEntities, totalCount: 2);

        await CreateHandler().Handle(CreateQuery(), CancellationToken.None);

        VerifyPageQuery(o =>
            o.OrderByDESC != null &&
            o.ThenByDESC != null &&
            o.AsSplitQuery &&
            o.AsNoTracking);
    }

    [Fact]
    public async Task Handle_ShouldUseSameFilterForCountAndPageQuery()
    {
        Expression<Func<EventNewsEntity, bool>>? countFilter = null;
        Expression<Func<EventNewsEntity, bool>>? pageFilter = null;
        SetUpDependencies(_eventNewsEntities, totalCount: 2);
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(o => countFilter = o.Filter)
            .ReturnsAsync(2);
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(o => pageFilter = o.Filter)
            .ReturnsAsync(_eventNewsEntities);

        await CreateHandler().Handle(CreateQuery(categoryId: 10), CancellationToken.None);

        Assert.NotNull(countFilter);
        Assert.NotNull(pageFilter);
        Assert.Same(countFilter, pageFilter);
    }

    [Fact]
    public async Task Handle_WithoutCategory_ShouldFilterOnlyPublishedItems()
    {
        var filter = await CaptureCountFilterAsync(CreateQuery());

        var matched = BuildFilterTestData().Where(filter.Compile()).Select(e => e.Id).ToList();

        Assert.Equal([1L, 2L, 3L], matched);
    }

    [Fact]
    public async Task Handle_WithCategory_ShouldFilterPublishedItemsOfThatCategory()
    {
        var filter = await CaptureCountFilterAsync(CreateQuery(categoryId: 10));

        var matched = BuildFilterTestData().Where(filter.Compile()).Select(e => e.Id).ToList();

        // 1 and 3 are published in category 10; 4 is a draft in category 10; 2 is in another category
        Assert.Equal([1L, 3L], matched);
    }

    private static GetPublishedEventNewsQuery CreateQuery(
        long? categoryId = null,
        int? offset = null,
        int? limit = null) => new(categoryId, offset, limit);

    private GetPublishedEventNewsHandler CreateHandler() =>
        new(_mapperMock.Object, _mockRepositoryWrapper.Object);

    private static List<EventNewsEntity> BuildFilterTestData() =>
    [
        new() { Id = 1, Status = Status.Published, Categories = [new EventNewsCategory { Id = 10 }] },
        new() { Id = 2, Status = Status.Published, Categories = [new EventNewsCategory { Id = 20 }] },
        new() { Id = 3, Status = Status.Published, Categories = [new EventNewsCategory { Id = 10 }, new EventNewsCategory { Id = 20 }] },
        new() { Id = 4, Status = Status.Draft, Categories = [new EventNewsCategory { Id = 10 }] },
        new() { Id = 5, Status = Status.Draft, Categories = [] },
    ];

    private async Task<Expression<Func<EventNewsEntity, bool>>> CaptureCountFilterAsync(
        GetPublishedEventNewsQuery query)
    {
        Expression<Func<EventNewsEntity, bool>>? captured = null;
        SetUpDependencies(_eventNewsEntities, totalCount: 2);
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(o => captured = o.Filter)
            .ReturnsAsync(2);

        await CreateHandler().Handle(query, CancellationToken.None);

        Assert.NotNull(captured);
        return captured;
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

    private void SetUpDependencies(IEnumerable<EventNewsEntity> items, int totalCount)
    {
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
            .Setup(x => x.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(items);
    }
}
