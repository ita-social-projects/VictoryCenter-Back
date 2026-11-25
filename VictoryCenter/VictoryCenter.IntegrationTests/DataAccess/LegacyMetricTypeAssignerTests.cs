using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.DataAccess;

// The previous backend version reads metrics by their legacy Type and expects each type at most once per block.
public class LegacyMetricTypeAssignerTests
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    [Fact]
    public async Task SaveChanges_NewBlock_GivesEachMetricAUniqueTypeMatchingItsKind()
    {
        await using var db = CreateContext();
        var statisticId = await AddBlockAsync(
            db,
            MetricKind.Standard,
            MetricKind.RaisedFunds,
            MetricKind.Standard,
            MetricKind.Standard);

        Assert.Equal(
            [LegacyMetricType.Partners, LegacyMetricType.Raised, LegacyMetricType.Programs, LegacyMetricType.TherapyHours],
            await LoadLegacyTypesAsync(statisticId));
    }

    [Fact]
    public void SaveChanges_Synchronous_AssignsTypesToo()
    {
        using var db = CreateContext();
        var statistics = NewBlock(MetricKind.RaisedFunds, MetricKind.Standard);
        db.MainPages.Add(NewMainPage(statistics));

        db.SaveChanges();

        Assert.Equal(
            [LegacyMetricType.Raised, LegacyMetricType.Partners],
            db.Metrics.OrderBy(m => m.Priority).Select(m => EF.Property<LegacyMetricType>(m, Metric.LegacyTypeProperty)));
    }

    [Fact]
    public async Task SaveChanges_MetricAddedToPersistedBlockNotLoaded_SkipsTypesStoredInTheDatabase()
    {
        long statisticId;
        await using (var db = CreateContext())
        {
            statisticId = await AddBlockAsync(db, MetricKind.Standard, MetricKind.RaisedFunds, MetricKind.Standard);
        }

        await using (var db = CreateContext())
        {
            db.Metrics.Add(NewMetric(MetricKind.Standard, priority: 10, statisticId));
            await db.SaveChangesAsync();
        }

        Assert.Equal(
            [LegacyMetricType.Partners, LegacyMetricType.Raised, LegacyMetricType.Programs, LegacyMetricType.TherapyHours],
            await LoadLegacyTypesAsync(statisticId));
    }

    [Fact]
    public async Task SaveChanges_MetricDeletedInTheSameSave_FreesItsType()
    {
        long statisticId;
        await using (var db = CreateContext())
        {
            statisticId = await AddBlockAsync(
                db,
                MetricKind.Standard,
                MetricKind.RaisedFunds,
                MetricKind.Standard,
                MetricKind.Standard);
        }

        await using (var db = CreateContext())
        {
            var partners = await db.Metrics.SingleAsync(m => m.StatisticId == statisticId && m.Priority == 0);
            db.Metrics.Remove(partners);
            db.Metrics.Add(NewMetric(MetricKind.Standard, priority: 10, statisticId));
            await db.SaveChangesAsync();
        }

        Assert.Equal(
            [LegacyMetricType.Raised, LegacyMetricType.Programs, LegacyMetricType.TherapyHours, LegacyMetricType.Partners],
            await LoadLegacyTypesAsync(statisticId));
    }

    [Fact]
    public async Task SaveChanges_PresetTypes_KeepsFreeOnesAndReplacesTakenOrInvalidOnes()
    {
        await using var db = CreateContext();
        var statistics = NewBlock(MetricKind.Standard, MetricKind.Standard, MetricKind.Standard, MetricKind.RaisedFunds);
        db.MainPages.Add(NewMainPage(statistics));
        var metrics = statistics.Metrics.ToList();
        SetLegacyType(db, metrics[0], LegacyMetricType.Raised); // invalid for Standard
        SetLegacyType(db, metrics[1], LegacyMetricType.TherapyHours); // free, kept
        SetLegacyType(db, metrics[2], LegacyMetricType.TherapyHours); // taken by the previous metric
        SetLegacyType(db, metrics[3], LegacyMetricType.Partners); // invalid for RaisedFunds

        await db.SaveChangesAsync();

        Assert.Equal(
            [LegacyMetricType.Partners, LegacyMetricType.TherapyHours, LegacyMetricType.Programs, LegacyMetricType.Raised],
            await LoadLegacyTypesAsync(statistics.Id));
    }

    [Fact]
    public async Task SaveChanges_SeparateBlocks_AreAssignedIndependently()
    {
        await using var db = CreateContext();
        var first = await AddBlockAsync(db, MetricKind.Standard, MetricKind.RaisedFunds);
        var second = await AddBlockAsync(db, MetricKind.Standard, MetricKind.RaisedFunds);

        Assert.Equal([LegacyMetricType.Partners, LegacyMetricType.Raised], await LoadLegacyTypesAsync(first));
        Assert.Equal([LegacyMetricType.Partners, LegacyMetricType.Raised], await LoadLegacyTypesAsync(second));
    }

    private static ImpactStatistics NewBlock(params MetricKind[] kinds) => new()
    {
        Title = "Statistics title",
        Metrics = [.. kinds.Select((kind, index) => NewMetric(kind, index))],
    };

    private static DAL.Entities.MainPage NewMainPage(ImpactStatistics statistics) => new()
    {
        Title = "Main page title",
        Description = "Main page description",
        ImpactStatistics = statistics,
    };

    private static Metric NewMetric(MetricKind kind, long priority, long statisticId = 0) => new()
    {
        Name = $"Metric {priority}",
        Kind = kind,
        Priority = priority,
        StatisticId = statisticId,
    };

    private static void SetLegacyType(VictoryCenterDbContext db, Metric metric, LegacyMetricType type) =>
        db.Entry(metric).Property(Metric.LegacyTypeProperty).CurrentValue = type;

    private static async Task<long> AddBlockAsync(VictoryCenterDbContext db, params MetricKind[] kinds)
    {
        var statistics = NewBlock(kinds);
        db.MainPages.Add(NewMainPage(statistics));
        await db.SaveChangesAsync();
        return statistics.Id;
    }

    private VictoryCenterDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options);

    private async Task<List<LegacyMetricType>> LoadLegacyTypesAsync(long statisticId)
    {
        await using var db = CreateContext();
        return await db.Metrics
            .Where(m => m.StatisticId == statisticId)
            .OrderBy(m => m.Priority)
            .Select(m => EF.Property<LegacyMetricType>(m, Metric.LegacyTypeProperty))
            .ToListAsync();
    }
}
