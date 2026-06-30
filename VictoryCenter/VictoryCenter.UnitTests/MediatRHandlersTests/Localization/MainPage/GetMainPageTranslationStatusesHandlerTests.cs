using Moq;
using VictoryCenter.BLL.Enums;
using VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetStatuses;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.MainPage;

public class GetMainPageTranslationStatusesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMainPageRepository> _mainPageRepositoryMock = new();

    public GetMainPageTranslationStatusesHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.MainPageRepository)
            .Returns(_mainPageRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnStatusesPerBlock_WhenMainPageExists()
    {
        // Arrange
        const long languageId = 2;
        var mainPage = new DAL.Entities.MainPage
        {
            Id = 1,
            Localizations =
            [
                new MainPageLocalization
                {
                    EntityId = 1,
                    LanguageId = languageId,
                    TranslationStatus = TranslationStatus.Relevant,
                },
            ],
            MainAboutUs = new MainAboutUs
            {
                Id = 10,
                Localizations =
                [
                    new MainAboutUsLocalization
                    {
                        EntityId = 10,
                        LanguageId = languageId,
                        TranslationStatus = TranslationStatus.Outdated,
                    },
                ],
            },
            MainPartners = new MainPartners
            {
                Id = 11,
                Localizations = [],
            },
            MainDonations = new MainDonations
            {
                Id = 12,
                Localizations =
                [
                    new MainDonationsLocalization
                    {
                        EntityId = 12,
                        LanguageId = languageId,
                        TranslationStatus = TranslationStatus.Relevant,
                    },
                ],
            },
            ImpactStatistics = new ImpactStatistics
            {
                Id = 13,
                Localizations =
                [
                    new ImpactStatisticsLocalization
                    {
                        EntityId = 13,
                        LanguageId = languageId,
                        TranslationStatus = TranslationStatus.Outdated,
                    },
                ],
                Metrics =
                [
                    new Metric
                    {
                        Id = 14,
                        Type = MetricType.Raised,
                        Localizations =
                        [
                            new MetricLocalization
                            {
                                EntityId = 14,
                                LanguageId = languageId,
                                TranslationStatus = TranslationStatus.Relevant,
                            },
                        ],
                    },
                ],
            },
        };

        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync(mainPage);

        var handler = new GetMainPageTranslationStatusesHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetMainPageTranslationStatusesQuery(1, languageId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value.Count);

        Assert.Equal(
            TranslationStatus.Relevant,
            result.Value.Single(s => s.Block == MainPageLocalizationBlock.Title).TranslationStatus);
        Assert.Equal(
            TranslationStatus.Outdated,
            result.Value.Single(s => s.Block == MainPageLocalizationBlock.AboutUs).TranslationStatus);
        Assert.Null(result.Value.Single(s => s.Block == MainPageLocalizationBlock.Partners).TranslationStatus);
        Assert.Equal(
            TranslationStatus.Relevant,
            result.Value.Single(s => s.Block == MainPageLocalizationBlock.Donations).TranslationStatus);
        Assert.Equal(
            TranslationStatus.Outdated,
            result.Value.Single(s => s.Block == MainPageLocalizationBlock.ImpactStatistics).TranslationStatus);
        Assert.Equal(
            TranslationStatus.Relevant,
            result.Value.Single(s => s.Block == MainPageLocalizationBlock.MetricRaised).TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMainPageDoesNotExist()
    {
        // Arrange
        _mainPageRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.MainPage>?>()))
            .ReturnsAsync((DAL.Entities.MainPage?)null);

        var handler = new GetMainPageTranslationStatusesHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(new GetMainPageTranslationStatusesQuery(1, 2), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }
}
