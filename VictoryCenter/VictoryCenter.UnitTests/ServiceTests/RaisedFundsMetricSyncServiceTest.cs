using Moq;
using VictoryCenter.BLL.Services.FundsMetricSync;

using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;

using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;

using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ServiceTests;

public class RaisedFundsMetricSyncServiceTests
{
    private const int EnglishLanguageId = 2;
    private const string EnglishLanguageCode = "en";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _reportFundsRecordsRepoMock = new();
    private readonly Mock<ILocalizationLanguagesRepository> _localizationLanguagesRepoMock = new();
    private readonly Mock<IMetricLocalizationsRepository> _metricLocalizationsRepoMock = new();
    private readonly Mock<TimeProvider> _timeProviderMock = new();
    private readonly RaisedFundsMetricSyncService _service;

    public RaisedFundsMetricSyncServiceTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.ReportFundsExpendituresRecordsRepository)
            .Returns(_reportFundsRecordsRepoMock.Object);

        _repositoryWrapperMock
            .SetupGet(x => x.LocalizationLanguagesRepository)
            .Returns(_localizationLanguagesRepoMock.Object);

        _repositoryWrapperMock
            .SetupGet(x => x.MetricLocalizationsRepository)
            .Returns(_metricLocalizationsRepoMock.Object);

        _service = new RaisedFundsMetricSyncService(_repositoryWrapperMock.Object, _timeProviderMock.Object);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldReturnFalseAndDoNothing_WhenMetricTypeIsNotRaised()
    {
        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Partners,
            IsAutoSynced = true,
            Value = 100
        };

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.False(result);
        Assert.Equal(100, metric.Value);
        _reportFundsRecordsRepoMock.Verify(x => x.GetSummaryAsync(), Times.Never);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldReturnFalseAndDoNothing_WhenIsAutoSyncedIsFalse()
    {
        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = false,
            Value = 100
        };

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.False(result);
        Assert.Equal(100, metric.Value);
        _reportFundsRecordsRepoMock.Verify(x => x.GetSummaryAsync(), Times.Never);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldUpdateValueAndEnglishLocalization_WhenSummaryValuesDiffer()
    {
        var existingEnglishLoc = new MetricLocalization
        {
            LanguageId = EnglishLanguageId,
            Value = "50",
            TranslationStatus = TranslationStatus.Outdated
        };

        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 1000,
            Localizations = new List<MetricLocalization> { existingEnglishLoc }
        };

        SetupReportSummary(incomeUahTotal: 5000.40m, incomeUsdTotal: 120.75m);
        SetupEnglishLanguageInDb();

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(5000, metric.Value);
        Assert.Equal("120.75", existingEnglishLoc.Value);
        Assert.Equal(TranslationStatus.Outdated, existingEnglishLoc.TranslationStatus);
        _metricLocalizationsRepoMock.Verify(x => x.CreateAsync(It.IsAny<MetricLocalization>()), Times.Never);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldReturnFalse_WhenValuesAlreadyMatchSummary()
    {
        var existingEnglishLoc = new MetricLocalization
        {
            LanguageId = EnglishLanguageId,
            Value = "120",
            TranslationStatus = TranslationStatus.Relevant
        };

        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 5000,
            Localizations = new List<MetricLocalization> { existingEnglishLoc }
        };

        SetupReportSummary(incomeUahTotal: 5000.00m, incomeUsdTotal: 120m);
        SetupEnglishLanguageInDb();

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.False(result);
        Assert.Equal(5000, metric.Value);
        Assert.Equal("120", existingEnglishLoc.Value);
        _metricLocalizationsRepoMock.Verify(x => x.CreateAsync(It.IsAny<MetricLocalization>()), Times.Never);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldCreateEnglishLocalization_WhenItDoesNotExist()
    {
        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 100,
            Localizations = new List<MetricLocalization>
            {
                new() { LanguageId = 1, Value = "100" }
            }
        };

        SetupReportSummary(incomeUahTotal: 4743042m, incomeUsdTotal: 114705m);
        SetupEnglishLanguageInDb();

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(4743042, metric.Value);

        _metricLocalizationsRepoMock.Verify(
            x => x.CreateAsync(It.Is<MetricLocalization>(l =>
                l.EntityId == metric.Id &&
                l.LanguageId == EnglishLanguageId &&
                l.Value == "114705" &&
                l.TranslationStatus == TranslationStatus.Relevant)),
            Times.Once);
    }

    [Theory]
    [InlineData(100.5, 101)]
    [InlineData(100.4, 100)]
    [InlineData(100.6, 101)]
    public async Task ApplySyncAsync_ShouldRoundUahCorrectly_UsingAwayFromZero(decimal uahInput, int expectedRounded)
    {
        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 0,
            Localizations = new List<MetricLocalization>()
        };

        SetupReportSummary(incomeUahTotal: uahInput, incomeUsdTotal: 10m);
        SetupEnglishLanguageInDb();

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(expectedRounded, metric.Value);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldClampToIntMax_WhenIncomeExceedsIntMaxValue()
    {
        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 0,
            Localizations = new List<MetricLocalization>()
        };

        var hugeIncome = int.MaxValue + 5000m;
        SetupReportSummary(incomeUahTotal: hugeIncome, incomeUsdTotal: 100m);
        SetupEnglishLanguageInDb();

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(int.MaxValue, metric.Value);
    }

    [Fact]
    public async Task ApplySyncAsync_ShouldUpdateUahOnly_WhenEnglishLanguageNotFoundInDb()
    {
        var metric = new Metric
        {
            Id = 1,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 100,
            Localizations = new List<MetricLocalization>()
        };

        SetupReportSummary(incomeUahTotal: 2000m, incomeUsdTotal: 50m);

        _localizationLanguagesRepoMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync((LocalizationLanguage?)null);

        var result = await _service.ApplySyncAsync(metric, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(2000, metric.Value);
        _metricLocalizationsRepoMock.Verify(x => x.CreateAsync(It.IsAny<MetricLocalization>()), Times.Never);
    }

    private void SetupReportSummary(decimal incomeUahTotal, decimal incomeUsdTotal)
    {
        _reportFundsRecordsRepoMock
            .Setup(x => x.GetSummaryAsync())
            .ReturnsAsync((
                incomeUahTotal,
                incomeUsdTotal,
                1,
                0m,
                0m,
                1
            ));
    }

    private void SetupEnglishLanguageInDb()
    {
        _localizationLanguagesRepoMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage
            {
                Id = EnglishLanguageId,
                Code = EnglishLanguageCode
            });
    }
}
