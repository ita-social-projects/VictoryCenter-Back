using Moq;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Notifications.ReportFunds;

public class SyncRaisedFundsMetricHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock = new();
    private readonly Mock<IMetricRepository> _metricRepositoryMock = new();
    private readonly Mock<ILocalizationLanguagesRepository> _languagesRepositoryMock = new();
    private readonly Mock<IMetricLocalizationsRepository> _metricLocalizationsRepositoryMock = new();

    public SyncRaisedFundsMetricHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.MetricRepository)
            .Returns(_metricRepositoryMock.Object);
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.LocalizationLanguagesRepository)
            .Returns(_languagesRepositoryMock.Object);
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.MetricLocalizationsRepository)
            .Returns(_metricLocalizationsRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ReturnsAsync(1);
        _metricLocalizationsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<MetricLocalization>()))
            .ReturnsAsync((MetricLocalization localization) => localization);
    }

    [Fact]
    public async Task Handle_ShouldUpdateRaisedMetricAndExistingEnglishLocalization_WhenAutoSynced()
    {
        // Arrange
        var englishLanguage = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var metric = new Metric
        {
            Id = 10,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 1,
            Name = "raised",
            Localizations =
            [
                new MetricLocalization
                {
                    EntityId = 10,
                    LanguageId = englishLanguage.Id,
                    Language = englishLanguage,
                    Value = "old",
                    TranslationStatus = TranslationStatus.Outdated,
                },
            ],
        };

        SetupSummary(123.5m, 45.6m);
        SetupRaisedMetric(metric);
        SetupLanguages([englishLanguage]);

        var handler = CreateHandler();

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        Assert.Equal(124, metric.Value);

        var englishLocalization = metric.Localizations.Single();
        Assert.Equal("45.6", englishLocalization.Value);
        Assert.Equal(TranslationStatus.Relevant, englishLocalization.TranslationStatus);

        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
        _metricLocalizationsRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<MetricLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSaveMetricValue_WhenEnglishLanguageDoesNotExist()
    {
        // Arrange
        var metric = new Metric
        {
            Id = 10,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 1,
            Name = "raised",
            Localizations = [],
        };

        SetupSummary(321.4m, 75.2m);
        SetupRaisedMetric(metric);
        SetupLanguages([]);

        var handler = CreateHandler();

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        Assert.Equal(321, metric.Value);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
        _metricLocalizationsRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<MetricLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateEnglishLocalization_WhenItDoesNotExist()
    {
        // Arrange
        var englishLanguage = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" };
        var metric = new Metric
        {
            Id = 10,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 1,
            Name = "raised",
            Localizations = [],
        };

        SetupSummary(99.5m, 78.9m);
        SetupRaisedMetric(metric);
        SetupLanguages([
            new LocalizationLanguage { Id = 1, Code = "uk", Name = "Ukrainian" },
            englishLanguage,
        ]);

        var handler = CreateHandler();

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        Assert.Equal(100, metric.Value);
        _metricLocalizationsRepositoryMock.Verify(
            repository => repository.CreateAsync(It.Is<MetricLocalization>(localization =>
                localization.EntityId == metric.Id
                && localization.LanguageId == englishLanguage.Id
                && localization.Value == "78.9")),
            Times.Once);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenRaisedMetricIsNotAutoSynced()
    {
        // Arrange
        var metric = new Metric
        {
            Id = 10,
            Type = MetricType.Raised,
            IsAutoSynced = false,
            Value = 1,
            Name = "raised",
            Localizations = [],
        };

        SetupSummary(123.5m, 45.6m);
        SetupRaisedMetric(metric);

        var handler = CreateHandler();

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        Assert.Equal(1, metric.Value);
        _languagesRepositoryMock.Verify(
            repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()),
            Times.Never);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    private SyncRaisedFundsMetricHandler CreateHandler() => new(_repositoryWrapperMock.Object);

    private void SetupSummary(decimal incomeUahTotal, decimal incomeUsdTotal)
    {
        _recordsRepositoryMock
            .Setup(repository => repository.GetSummaryAsync())
            .ReturnsAsync((incomeUahTotal, incomeUsdTotal, 0, 0, 0, 0));
    }

    private void SetupRaisedMetric(Metric? metric)
    {
        _metricRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>>()))
            .ReturnsAsync((QueryOptions<Metric>? options) =>
            {
                if (metric is null)
                {
                    return null;
                }

                var filter = options?.Filter;
                return filter is null || filter.Compile()(metric)
                    ? metric
                    : null;
            });
    }

    private void SetupLanguages(IEnumerable<LocalizationLanguage> languages)
    {
        _languagesRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((QueryOptions<LocalizationLanguage>? options) =>
            {
                var filter = options?.Filter;
                return filter is null
                    ? languages.FirstOrDefault()
                    : languages.FirstOrDefault(filter.Compile());
            });
    }
}
