using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage;

// Runs on the in-memory provider, so it also runs in CI; MainPageInitializationTests covers the
// locking startup path against SQL Server.
public class MainPageMetricSeedingTests
{
    private const string PartnersName = "Партнерів";
    private const string ProgramsName = "Програм";
    private const string RaisedName = "Зібрано";
    private const string TherapyHoursName = "Годин терапії";

    private static readonly MethodInfo SeedMissingMetricsMethod = typeof(ServicesConfiguration)
        .GetMethod("SeedMissingMetricsAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly Mock<ILogger> _logger = new();

    [Fact]
    public async Task SeedMissingMetrics_EmptyBlock_AddsAllFourDefaultsInOrder()
    {
        await using var db = CreateContext();
        var statistics = await SeedStatisticsAsync(db);

        await SeedMissingMetricsAsync(db, statistics);

        var metrics = await LoadMetricsAsync(db);
        Assert.Equal([PartnersName, ProgramsName, RaisedName, TherapyHoursName], metrics.Select(m => m.Name));
        Assert.Equal(
            [MetricKind.Standard, MetricKind.Standard, MetricKind.RaisedFunds, MetricKind.Standard],
            metrics.Select(m => m.Kind));
        Assert.Equal([0L, 1L, 2L, 3L], metrics.Select(m => m.Priority));
    }

    [Fact]
    public async Task SeedMissingMetrics_IncompleteBlock_AddsUnusedDefaultNamesAfterExistingPriorities()
    {
        await using var db = CreateContext();
        var statistics = await SeedStatisticsAsync(
            db,
            (PartnersName, MetricKind.Standard, 5),
            (RaisedName, MetricKind.RaisedFunds, 7));

        await SeedMissingMetricsAsync(db, statistics);

        var metrics = await LoadMetricsAsync(db);
        Assert.Equal(4, metrics.Count);
        var added = metrics.Where(m => m.Priority > 7).ToList();
        Assert.Equal([ProgramsName, TherapyHoursName], added.Select(m => m.Name));
        Assert.Equal([8L, 9L], added.Select(m => m.Priority));
        Assert.All(added, metric => Assert.Equal(MetricKind.Standard, metric.Kind));
    }

    [Fact]
    public async Task SeedMissingMetrics_CompleteBlock_AddsNothing()
    {
        await using var db = CreateContext();
        var statistics = await SeedStatisticsAsync(
            db,
            ("A", MetricKind.Standard, 0),
            ("B", MetricKind.Standard, 1),
            ("C", MetricKind.RaisedFunds, 2),
            ("D", MetricKind.Standard, 3));

        await SeedMissingMetricsAsync(db, statistics);

        Assert.Equal(["A", "B", "C", "D"], (await LoadMetricsAsync(db)).Select(m => m.Name));
    }

    [Theory]
    [InlineData(MetricKind.Standard, MetricKind.Standard)]
    [InlineData(MetricKind.RaisedFunds, MetricKind.RaisedFunds)]
    public async Task SeedMissingMetrics_BlockWithExcessMetricsOfOneKind_AddsNothingAndLogsWarning(
        MetricKind firstKind,
        MetricKind thirdKind)
    {
        // 4 Standard, or 2 Standard + 2 RaisedFunds.
        await using var db = CreateContext();
        var statistics = await SeedStatisticsAsync(
            db,
            ("A", firstKind, 0),
            ("B", MetricKind.Standard, 1),
            ("C", thirdKind, 2),
            ("D", MetricKind.Standard, 3));

        await SeedMissingMetricsAsync(db, statistics);

        Assert.Equal(["A", "B", "C", "D"], (await LoadMetricsAsync(db)).Select(m => m.Name));
        _logger.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains($"block {statistics.Id} ")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private Task SeedMissingMetricsAsync(VictoryCenterDbContext db, ImpactStatistics statistics) =>
        (Task)SeedMissingMetricsMethod.Invoke(null, [db, statistics, TimeProvider.System, _logger.Object])!;

    private static VictoryCenterDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<ImpactStatistics> SeedStatisticsAsync(
        VictoryCenterDbContext db,
        params (string Name, MetricKind Kind, long Priority)[] metrics)
    {
        var statistics = new ImpactStatistics
        {
            Title = "Statistics title",
            Metrics = [.. metrics.Select(m => new Metric { Name = m.Name, Kind = m.Kind, Priority = m.Priority })],
        };
        db.MainPages.Add(new DAL.Entities.MainPage
        {
            Title = "Main page title",
            Description = "Main page description",
            ImpactStatistics = statistics,
        });
        await db.SaveChangesAsync();
        return statistics;
    }

    private static async Task<List<Metric>> LoadMetricsAsync(VictoryCenterDbContext db) =>
        await db.Metrics.AsNoTracking().OrderBy(m => m.Priority).ToListAsync();
}
