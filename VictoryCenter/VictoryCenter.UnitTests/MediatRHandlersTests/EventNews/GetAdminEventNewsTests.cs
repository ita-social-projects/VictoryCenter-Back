using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Mapping.EventNews;
using VictoryCenter.BLL.Mapping.EventNewsCategories;
using VictoryCenter.BLL.Mapping.Localization.Languages;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews;

public class GetAdminEventNewsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();

    [Fact]
    public void Mapping_MapsEventAndCategoryLocalizationsWithLanguages()
    {
        var language = new LocalizationLanguage { Id = 1, Code = "uk", Name = "Ukrainian" };
        var entity = new EventNewsEntity
        {
            Id = 10,
            Categories =
            [
                new EventNewsCategory
                {
                    Id = 2,
                    Name = "Events",
                    Localizations =
                    [
                        new EventNewsCategoryLocalization
                        {
                            EntityId = 2,
                            LanguageId = language.Id,
                            Language = language,
                            Name = "Localized events",
                            TranslationStatus = TranslationStatus.Relevant
                        },
                    ]
                },
            ],
            Localizations =
            [
                new EventNewsLocalization
                {
                    EntityId = 10,
                    LanguageId = language.Id,
                    Language = language,
                    Title = "Event title",
                    Description = "Event description",
                    TranslationStatus = TranslationStatus.Outdated
                },
            ]
        };
        var configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<EventNewsProfile>();
            config.AddProfile<EventNewsCategoryProfile>();
            config.AddProfile<LocalizationsLanguageProfile>();
        });
        var mapper = configuration.CreateMapper();

        var result = mapper.Map<EventNewsDto>(entity);

        var category = Assert.Single(result.Categories);
        var categoryLocalization = Assert.Single(category.Localizations);
        Assert.Equal("Localized events", categoryLocalization.Name);
        Assert.Equal("uk", categoryLocalization.Language.Code);
        var eventLocalization = Assert.Single(result.Localizations);
        Assert.Equal("Event title", eventLocalization.Title);
        Assert.Equal("uk", eventLocalization.Language.Code);
        Assert.Equal(TranslationStatus.Outdated, eventLocalization.TranslationStatus);
    }

    [Fact]
    public async Task GetByFilters_ReturnsMappedPageAndMatchingTotalCount()
    {
        var entities = new[] { new EventNewsEntity { Id = 3 }, new EventNewsEntity { Id = 2 } };
        var mappedItems = new[] { new EventNewsDto { Id = 3 }, new EventNewsDto { Id = 2 } };
        QueryOptions<EventNewsEntity>? listOptions = null;
        QueryOptions<EventNewsEntity>? countOptions = null;

        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(options => listOptions = options)
            .ReturnsAsync(entities);
        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(options => countOptions = options)
            .ReturnsAsync(7);
        _mapper.Setup(mapper => mapper.Map<EventNewsDto[]>(entities)).Returns(mappedItems);

        var handler = new GetEventNewsByFiltersHandler(_mapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(
            new GetEventNewsByFiltersQuery(new EventNewsFilterDto { Offset = 1, Limit = 2 }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(mappedItems, result.Value.Items);
        Assert.Equal(7, result.Value.TotalItemsCount);
        Assert.NotNull(listOptions);
        Assert.Equal(1, listOptions.Offset);
        Assert.Equal(2, listOptions.Limit);
        Assert.True(listOptions.AsNoTracking);
        Assert.NotNull(listOptions.Include);
        Assert.NotNull(listOptions.OrderByDESC);
        Assert.Equal(3L, Assert.IsType<long>(listOptions.OrderByDESC.Compile()(entities[0])));
        Assert.NotNull(countOptions);
        Assert.True(countOptions.AsNoTracking);
        Assert.Equal(0, countOptions.Offset);
        Assert.Equal(0, countOptions.Limit);
        Assert.Null(countOptions.Include);
        Assert.Null(countOptions.OrderByDESC);
    }

    [Fact]
    public async Task GetByFilters_AppliesCategoryFilterToListAndCountQueries()
    {
        var matchingCategory = new EventNewsCategory { Id = 5, Name = "Events" };
        var matchingItem = new EventNewsEntity { Id = 1, Categories = [matchingCategory] };
        var otherItem = new EventNewsEntity
        {
            Id = 2,
            Categories = [new EventNewsCategory { Id = 6, Name = "News" }]
        };
        QueryOptions<EventNewsEntity>? listOptions = null;
        QueryOptions<EventNewsEntity>? countOptions = null;

        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(options => listOptions = options)
            .ReturnsAsync([matchingItem]);
        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(options => countOptions = options)
            .ReturnsAsync(1);
        _mapper.Setup(mapper => mapper.Map<EventNewsDto[]>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns([new EventNewsDto { Id = 1 }]);

        var handler = new GetEventNewsByFiltersHandler(_mapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(
            new GetEventNewsByFiltersQuery(new EventNewsFilterDto { CategoryId = 5 }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(listOptions?.Filter);
        Assert.NotNull(countOptions?.Filter);
        Assert.True(listOptions.Filter.Compile()(matchingItem));
        Assert.False(listOptions.Filter.Compile()(otherItem));
        Assert.True(countOptions.Filter.Compile()(matchingItem));
        Assert.False(countOptions.Filter.Compile()(otherItem));
    }

    [Fact]
    public async Task GetByFilters_WhenNoItemsMatch_ReturnsEmptyPage()
    {
        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync([]);
        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.CountAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(0);
        _mapper.Setup(mapper => mapper.Map<EventNewsDto[]>(It.IsAny<IEnumerable<EventNewsEntity>>()))
            .Returns([]);

        var handler = new GetEventNewsByFiltersHandler(_mapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(
            new GetEventNewsByFiltersQuery(new EventNewsFilterDto { CategoryId = 99 }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalItemsCount);
    }

    [Fact]
    public async Task GetById_WhenItemExists_ReturnsMappedItemUsingReadOnlyQuery()
    {
        var entity = new EventNewsEntity { Id = 10 };
        var dto = new EventNewsDto { Id = 10 };
        QueryOptions<EventNewsEntity>? queryOptions = null;

        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(options => queryOptions = options)
            .ReturnsAsync(entity);
        _mapper.Setup(mapper => mapper.Map<EventNewsDto>(entity)).Returns(dto);

        var handler = new GetEventNewsByIdHandler(_mapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(new GetEventNewsByIdQuery(10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(dto, result.Value);
        Assert.NotNull(queryOptions);
        Assert.True(queryOptions.AsNoTracking);
        Assert.NotNull(queryOptions.Include);
        Assert.NotNull(queryOptions.Filter);
        Assert.True(queryOptions.Filter.Compile()(entity));
        Assert.False(queryOptions.Filter.Compile()(new EventNewsEntity { Id = 11 }));
    }

    [Fact]
    public async Task GetById_WhenItemDoesNotExist_ReturnsNotFound()
    {
        _repositoryWrapper
            .Setup(wrapper => wrapper.EventNewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync((EventNewsEntity?)null);

        var handler = new GetEventNewsByIdHandler(_mapper.Object, _repositoryWrapper.Object);
        var result = await handler.Handle(new GetEventNewsByIdQuery(404), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(404, typeof(EventNewsEntity)),
            result.Errors.Single().Message);
        _mapper.Verify(mapper => mapper.Map<EventNewsDto>(It.IsAny<EventNewsEntity>()), Times.Never);
    }
}
