using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Data;

/// <summary>
/// Gives every new metric a legacy Type that matches its kind and is unique within its block, as the previous
/// backend version expects. A type set on the metric before saving is kept when it is valid and still free.
/// Remove together with the legacy Type column.
/// </summary>
internal static class LegacyMetricTypeAssigner
{
    private static readonly LegacyMetricType[] StandardTypes =
        [LegacyMetricType.Partners, LegacyMetricType.Programs, LegacyMetricType.TherapyHours];

    private static readonly LegacyMetricType[] RaisedFundsTypes = [LegacyMetricType.Raised];

    public static void Assign(VictoryCenterDbContext context)
    {
        foreach (var block in GetBlocksWithNewMetrics(context))
        {
            var usedTypes = block.PersistedStatisticId is { } statisticId
                ? QueryPersistedTypes(context, statisticId, block.DeletedMetricIds).ToList()
                : [];
            AssignTypes(block.NewMetrics, usedTypes);
        }
    }

    public static async Task AssignAsync(VictoryCenterDbContext context, CancellationToken cancellationToken)
    {
        foreach (var block in GetBlocksWithNewMetrics(context))
        {
            var usedTypes = block.PersistedStatisticId is { } statisticId
                ? await QueryPersistedTypes(context, statisticId, block.DeletedMetricIds).ToListAsync(cancellationToken)
                : [];
            AssignTypes(block.NewMetrics, usedTypes);
        }
    }

    private static List<Block> GetBlocksWithNewMetrics(VictoryCenterDbContext context)
    {
        var metrics = context.ChangeTracker.Entries<Metric>().ToList();
        var deletedMetricIds = metrics
            .Where(entry => entry.State == EntityState.Deleted)
            .Select(entry => entry.Entity.Id)
            .ToList();

        return metrics
            .Where(entry => entry.State == EntityState.Added)
            .GroupBy(entry => (object?)entry.Entity.Statistics ?? entry.Entity.StatisticId)
            .Select(block =>
            {
                var statistics = block.First().Entity.Statistics;
                long? persistedStatisticId = statistics is not null && context.Entry(statistics).State == EntityState.Added
                    ? null
                    : block.First().Entity.StatisticId;
                return new Block([.. block], persistedStatisticId, deletedMetricIds);
            })
            .ToList();
    }

    private static IQueryable<LegacyMetricType> QueryPersistedTypes(
        VictoryCenterDbContext context,
        long statisticId,
        List<long> deletedMetricIds)
    {
        return context.Metrics
            .Where(metric => metric.StatisticId == statisticId && !deletedMetricIds.Contains(metric.Id))
            .Select(metric => EF.Property<LegacyMetricType>(metric, Metric.LegacyTypeProperty));
    }

    private static void AssignTypes(List<EntityEntry<Metric>> newMetrics, List<LegacyMetricType> persistedTypes)
    {
        var usedTypes = persistedTypes.ToHashSet();
        var unassigned = new List<EntityEntry<Metric>>();

        // Keep the types that are already valid and free first, so that one metric falling back
        // to a free type cannot take the type another metric asked for.
        foreach (var entry in newMetrics.OrderBy(entry => entry.Entity.Priority))
        {
            var type = TypeOf(entry).CurrentValue;
            if (!AllowedTypes(entry).Contains(type) || !usedTypes.Add(type))
            {
                unassigned.Add(entry);
            }
        }

        foreach (var entry in unassigned)
        {
            var allowedTypes = AllowedTypes(entry);

            // No free type is left only in blocks that break the 3 Standard + 1 RaisedFunds rule, which validation rejects.
            TypeOf(entry).CurrentValue = allowedTypes.FirstOrDefault(allowed => !usedTypes.Contains(allowed), allowedTypes[0]);
            usedTypes.Add(TypeOf(entry).CurrentValue);
        }
    }

    private static LegacyMetricType[] AllowedTypes(EntityEntry<Metric> entry) =>
        entry.Entity.Kind == MetricKind.RaisedFunds ? RaisedFundsTypes : StandardTypes;

    private static PropertyEntry<Metric, LegacyMetricType> TypeOf(EntityEntry<Metric> entry) =>
        entry.Property<LegacyMetricType>(Metric.LegacyTypeProperty);

    private sealed record Block(
        List<EntityEntry<Metric>> NewMetrics,
        long? PersistedStatisticId,
        List<long> DeletedMetricIds);
}
