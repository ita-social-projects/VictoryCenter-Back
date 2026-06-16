using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Public.MainPage.GetLocalizedMainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class GetLocalizedMainPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public GetLocalizedMainPageHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.MainPageRepository)
            .Returns(_mainPageRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizedContentWithFallback_WhenLanguageIdProvided()
    {
        // Arrange
        const long languageId = 2;
        var mainPage = new DAL.Entities.MainPage
        {
            Id = 1,
            Title = "Український заголовок",
            Description = "Український опис",
            Localizations =
            [
                new MainPageLocalization
                {
                    EntityId = 1,
                    LanguageId = languageId,
                    Title = "English title",
                    Description = "English description",
                },
            ],
            MainAboutUs = new MainAboutUs
            {
                Id = 10,
                Title = "Про нас",
                Description = "Український опис про нас",
                Localizations =
                [
                    new MainAboutUsLocalization
                    {
                        EntityId = 10,
                        LanguageId = languageId,
                        Title = "About us",
                        Description = "About us description",
                    },
                ],
            },
            MainPartners = new MainPartners
            {
                Id = 11,
                Title = "Партнери",
                Description = "Український опис партнерів",
                Localizations = [],
            },
            MainDonations = new MainDonations
            {
                Id = 12,
                Title = "Донати",
                Description = "Український опис донатів",
                Localizations =
                [
                    new MainDonationsLocalization
                    {
                        EntityId = 12,
                        LanguageId = languageId,
                        Title = "Donations",
                        Description = "Donation description",
                    },
                ],
            },
            ImpactStatistics = new ImpactStatistics
            {
                Id = 13,
                Title = "Статистика",
                Localizations =
                [
                    new ImpactStatisticsLocalization
                    {
                        EntityId = 13,
                        LanguageId = languageId,
                        Title = "Impact statistics",
                    },
                ],
                Metrics =
                [
                    new Metric
                    {
                        Id = 14,
                        Name = "Зібрано",
                        Value = 100,
                        Type = MetricType.Raised,
                        Priority = 1,
                        Localizations =
                        [
                            new MetricLocalization
                            {
                                EntityId = 14,
                                LanguageId = languageId,
                                Name = "Raised",
                                Value = "$100",
                            },
                        ],
                    },
                    new Metric
                    {
                        Id = 15,
                        Name = "Партнери",
                        Value = 5,
                        Type = MetricType.Partners,
                        Priority = 2,
                        Localizations = [],
                    },
                ],
            },
        };

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(mainPage);

        var handler = new GetLocalizedMainPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetLocalizedMainPageQuery(languageId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(languageId, result.Value.LanguageId);
        Assert.Equal("English title", result.Value.Title);
        Assert.Equal("About us", result.Value.MainAboutUs!.Title);
        Assert.Equal("Партнери", result.Value.MainPartners!.Title);
        Assert.Equal("Donations", result.Value.MainDonations!.Title);
        Assert.Equal("Impact statistics", result.Value.ImpactStatistics!.Title);

        var localizedMetric = result.Value.ImpactStatistics.Metrics.Single(m => m.Id == 14);
        Assert.Equal("Raised", localizedMetric.Name);
        Assert.Equal("$100", localizedMetric.Value);

        var fallbackMetric = result.Value.ImpactStatistics.Metrics.Single(m => m.Id == 15);
        Assert.Equal("Партнери", fallbackMetric.Name);
        Assert.Equal("5", fallbackMetric.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnSourceContent_WhenLanguageIdIsNotProvided()
    {
        // Arrange
        var mainPage = new DAL.Entities.MainPage
        {
            Id = 1,
            Title = "Український заголовок",
            Description = "Український опис",
            Localizations =
            [
                new MainPageLocalization
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Title = "English title",
                    Description = "English description",
                },
            ],
        };

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(mainPage);

        var handler = new GetLocalizedMainPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetLocalizedMainPageQuery(null), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.LanguageId);
        Assert.Equal("Український заголовок", result.Value.Title);
        Assert.Equal("Український опис", result.Value.Description);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPageDoesNotExist()
    {
        // Arrange
        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync((DAL.Entities.MainPage?)null);

        var handler = new GetLocalizedMainPageHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetLocalizedMainPageQuery(2), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
    }
}
