using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Mapping.EventsPage;
using VictoryCenter.BLL.Mapping.EventNews;
using VictoryCenter.BLL.Mapping.Localization.Languages;
using VictoryCenter.BLL.Queries.Public.EventsPage.Get;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventsPage;

public class GetPublicEventsPageTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly EventsIntroSection _intro = new()
    {
        EventsBlockTitle = "<p>Що відбувалось</p>",
        PageDescription = "<p>Опис сторінки</p>"
    };

    private readonly List<EventNewsCategory> _categories =
    [
        new() { Id = 14, Name = "Новини" },
        new() { Id = 10, Name = "Медіа" },
    ];

    private QueryOptions<EventNewsCategory>? _capturedCategoryOptions;
    public GetPublicEventsPageTests()
    {
        _mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<EventsIntroSectionProfile>();
            config.AddProfile<EventNewsProfile>();
            config.AddProfile<LocalizationsLanguageProfile>();
        }).CreateMapper();
    }

    [Fact]
    public async Task Handle_ShouldReturnIntroAndMappedCategories()
    {
        SetUpDependencies(_intro, _categories);

        var result = await CreateHandler().Handle(new GetPublicEventsPageQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_intro.EventsBlockTitle, result.Value.EventsBlockTitle);
        Assert.Equal(_intro.PageDescription, result.Value.PageDescription);
        Assert.Equal(
            _categories.Select(c => (c.Id, c.Name)),
            result.Value.Categories.Select(c => (c.Id, c.Name)));
    }

    [Fact]
    public async Task Handle_WhenNoCategories_ShouldReturnEmptyCategories()
    {
        SetUpDependencies(_intro, []);

        var result = await CreateHandler().Handle(new GetPublicEventsPageQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Categories);
        Assert.Equal(_intro.EventsBlockTitle, result.Value.EventsBlockTitle);
    }

    [Fact]
    public async Task Handle_WhenIntroSectionIsMissing_ShouldReturnNotFoundWithoutLoadingCategories()
    {
        SetUpDependencies(null, _categories);

        var result = await CreateHandler().Handle(new GetPublicEventsPageQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _repositoryWrapperMock.Verify(
            x => x.EventNewsCategoryRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsCategory>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldQueryCategoriesAsNoTrackingOrderedByName()
    {
        SetUpDependencies(_intro, _categories);

        await CreateHandler().Handle(new GetPublicEventsPageQuery(), CancellationToken.None);

        Assert.NotNull(_capturedCategoryOptions?.Include);
        Assert.NotNull(_capturedCategoryOptions);
        Assert.True(_capturedCategoryOptions.AsNoTracking);
        Assert.NotNull(_capturedCategoryOptions.OrderByASC);

        var orderedNames = _categories
            .OrderBy(_capturedCategoryOptions.OrderByASC.Compile())
            .Select(c => c.Name);
        Assert.Equal(_categories.Select(c => c.Name).Order(), orderedNames);
    }

    [Fact]
    public async Task Handle_ShouldMapCategoryLocalizations()
    {
        var language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var category = new EventNewsCategory
        {
            Id = 10,
            Name = "Медіа",
            Localizations =
            [
                new EventNewsCategoryLocalization
                {
                    EntityId = 10,
                    LanguageId = language.Id,
                    Language = language,
                    Name = "Media"
                },
            ]
        };
        SetUpDependencies(_intro, [category]);

        var result = await CreateHandler().Handle(new GetPublicEventsPageQuery(), CancellationToken.None);

        var localization = Assert.Single(Assert.Single(result.Value.Categories).Localizations);
        Assert.Equal("Media", localization.Name);
        Assert.Equal("en", localization.Language.Code);
    }

    private GetPublicEventsPageHandler CreateHandler() =>
        new(_repositoryWrapperMock.Object, _mapper);

    private void SetUpDependencies(EventsIntroSection? intro, List<EventNewsCategory> categories)
    {
        _repositoryWrapperMock
            .Setup(x => x.EventsIntroSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<EventsIntroSection>>()))
            .ReturnsAsync(intro);

        _repositoryWrapperMock
            .Setup(x => x.EventNewsCategoryRepository.GetAllAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .Callback<QueryOptions<EventNewsCategory>>(options => _capturedCategoryOptions = options)
            .ReturnsAsync(categories);
    }
}
