using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.StorageLifecycle;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.Migrations;

public class AddMetricKindMigrationTests : IAsyncLifetime
{
    private const string DatabasePrefix = "VictoryCenter_MetricKindMigrationTests_";
    private const string PreviousMigration = "_AddAuditLog";
    private const string TargetMigration = "_AddMetricKind";

    // Legacy MetricType values.
    private const int Partners = 0;
    private const int Programs = 1;
    private const int Raised = 2;
    private const int TherapyHours = 3;

    private static readonly MethodInfo SeedMissingMetricsMethod = typeof(ServicesConfiguration)
        .GetMethod("SeedMissingMetricsAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly string _databaseName = DatabasePrefix + Guid.NewGuid().ToString("N");
    private string _connectionString = null!;
    private DbContextOptions<VictoryCenterDbContext> _databaseOptions = null!;
    private string _previous = null!;
    private string _target = null!;

    public async Task InitializeAsync()
    {
        _connectionString = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(StorageSqlServerFactAttribute.ConnectionVariable))
        {
            InitialCatalog = _databaseName,
            ConnectTimeout = 10
        }.ConnectionString;
        _databaseOptions = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer(_connectionString, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "entity_framework"))
            .Options;

        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(StorageSqlServerFactAttribute.ConnectionVariable)))
        {
            return;
        }

        await using var context = Context();
        var migrations = context.Database.GetMigrations().ToArray();
        _previous = migrations.Single(migration => migration.EndsWith(PreviousMigration, StringComparison.Ordinal));
        _target = migrations.Single(migration => migration.EndsWith(TargetMigration, StringComparison.Ordinal));
        await context.GetService<IMigrator>().MigrateAsync(_previous);
    }

    [StorageSqlServerFact]
    public async Task Up_KeepsTypeAndComputesKindFromIt()
    {
        var ids = await SeedStatisticAsync(TherapyHours, Raised, Partners, Programs);

        await MigrateAsync(_target);

        Assert.Equal(new[] { TherapyHours, Raised, Partners, Programs }, await GetValuesAsync("Type", ids));
        Assert.Equal(new[] { 0, 1, 0, 0 }, await GetValuesAsync("Kind", ids));
    }

    [StorageSqlServerFact]
    public async Task UpAndDown_PreserveMetricDataAndLocalizations()
    {
        var ids = await SeedStatisticAsync(TherapyHours, Raised, Partners, Programs);
        await SetNonDefaultMetricDataAsync(ids);
        var before = await SnapshotAsync();
        Assert.Equal(8, before.Length); // 4 metrics and 4 localizations, so the comparison cannot pass vacuously.

        await MigrateAsync(_target);
        var afterUp = await SnapshotAsync();

        await MigrateAsync(_previous);
        var afterDown = await SnapshotAsync();

        Assert.Equal(before, afterUp);
        Assert.Equal(before, afterDown);
        Assert.False(await KindColumnExistsAsync());
    }

    [StorageSqlServerFact]
    public async Task Kind_FollowsWritesThatOnlyKnowType()
    {
        var ids = await SeedStatisticAsync(Partners, Raised);
        await MigrateAsync(_target);

        await using (var context = Context())
        {
            await context.Database.ExecuteSqlRawAsync("UPDATE Metrics SET Type = {0} WHERE Id = {1}", Programs, ids[0]);
            await context.Database.ExecuteSqlRawAsync("UPDATE Metrics SET Type = {0} WHERE Id = {1}", TherapyHours, ids[1]);
        }

        Assert.Equal(new[] { 0, 0 }, await GetValuesAsync("Kind", ids));
    }

    [StorageSqlServerFact]
    public async Task PreviousBackendModel_CanReadInsertAndUpdateMetricsAfterMigrationAndSeeding()
    {
        // A legacy block with two metrics; startup seeding adds Партнерів (Partners) and Програм, which gets
        // TherapyHours because Programs is taken.
        var ids = await SeedStatisticAsync(Raised, Programs);
        var statisticId = await GetStatisticIdAsync(ids[0]);
        await MigrateAsync(_target);
        await using (var context = Context())
        {
            var statistics = await context.ImpactStatistics.Include(s => s.Metrics).SingleAsync(s => s.Id == statisticId);
            await (Task)SeedMissingMetricsMethod.Invoke(
                null,
                [context, statistics, TimeProvider.System, NullLogger.Instance])!;
        }

        // Read: the previous backend indexes a block's metrics by type, so every type must appear once.
        await using (var legacy = LegacyContext())
        {
            var byType = (await legacy.Metrics.Where(m => m.StatisticId == statisticId).ToListAsync())
                .ToDictionary(m => m.Type);
            Assert.Equal(Enum.GetValues<LegacyMetricType>().Order(), byType.Keys.Order());

            // Insert a replacement metric and change existing ones the way the previous backend's update did.
            legacy.Metrics.Remove(byType[LegacyMetricType.Partners]);
            legacy.Metrics.Add(new LegacyMetric
            {
                StatisticId = statisticId,
                Name = "Inserted by previous backend",
                Value = 5,
                Type = LegacyMetricType.Partners,
                Priority = 20,
                CreatedAt = DateTimeOffset.UtcNow,
            });
            byType[LegacyMetricType.Raised].Type = LegacyMetricType.Programs;
            byType[LegacyMetricType.Programs].Type = LegacyMetricType.Raised;
            byType[LegacyMetricType.TherapyHours].Value = 777;
            await legacy.SaveChangesAsync();
        }

        await using (var context = Context())
        {
            var metrics = await context.Metrics.AsNoTracking()
                .Where(m => m.StatisticId == statisticId)
                .ToDictionaryAsync(m => m.Name);
            Assert.Equal(4, metrics.Count);
            Assert.Equal(MetricKind.Standard, metrics["Inserted by previous backend"].Kind);
            Assert.Equal(MetricKind.Standard, metrics["Metric 0"].Kind); // was Raised
            Assert.Equal(MetricKind.RaisedFunds, metrics["Metric 1"].Kind); // was Programs
            Assert.Equal(777, metrics["Програм"].Value);
        }
    }

    public async Task DisposeAsync()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(StorageSqlServerFactAttribute.ConnectionVariable)))
        {
            return;
        }

        await using var context = Context();
        var target = new SqlConnectionStringBuilder(context.Database.GetConnectionString());
        if (target.InitialCatalog != _databaseName || !_databaseName.StartsWith(DatabasePrefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Refusing to delete a database outside the generated test database name.");
        }

        await context.Database.EnsureDeletedAsync();
    }

    private VictoryCenterDbContext Context() => new(_databaseOptions);

    private LegacyMetricsContext LegacyContext() =>
        new(new DbContextOptionsBuilder<LegacyMetricsContext>().UseSqlServer(_connectionString).Options);

    private async Task MigrateAsync(string migration)
    {
        await using var context = Context();
        await context.GetService<IMigrator>().MigrateAsync(migration);
    }

    private async Task<long[]> SeedStatisticAsync(params int[] typesInPriorityOrder)
    {
        await using var context = Context();
        var mainPageId = await InsertAsync(
            context,
            "INSERT INTO MainPages (Title, Description, CreatedAt) OUTPUT INSERTED.Id AS Value VALUES (N'Main', N'Description', SYSDATETIMEOFFSET())");
        var statisticId = await InsertAsync(
            context,
            "INSERT INTO ImpactStatistics (MainPageId, Title, CreatedAt) OUTPUT INSERTED.Id AS Value VALUES ({0}, N'Impact', SYSDATETIMEOFFSET())",
            mainPageId);

        var ids = new long[typesInPriorityOrder.Length];
        for (var i = 0; i < typesInPriorityOrder.Length; i++)
        {
            ids[i] = await InsertAsync(
                context,
                "INSERT INTO Metrics (StatisticId, Name, Value, Type, Priority, CreatedAt) OUTPUT INSERTED.Id AS Value VALUES ({0}, {1}, {2}, {3}, {4}, SYSDATETIMEOFFSET())",
                statisticId,
                $"Metric {i}",
                i,
                typesInPriorityOrder[i],
                (long)(i + 1));
        }

        return ids;
    }

    private async Task SetNonDefaultMetricDataAsync(long[] ids)
    {
        await using var context = Context();
        var languageId = await InsertAsync(
            context,
            "INSERT INTO LocalizationLanguages (Code, Name, CreatedAt) OUTPUT INSERTED.Id AS Value VALUES (N'zz', N'Test language', SYSDATETIMEOFFSET())");

        for (var i = 0; i < ids.Length; i++)
        {
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE Metrics SET Value = {0}, Prefix = {1}, Priority = {2}, IsHidden = {3}, IsAutoSynced = {4} WHERE Id = {5}",
                1000 + (i * 37),
                (i % 2) + 1,
                (long)(40 - (i * 9)),
                i == 0,
                i == 1,
                ids[i]);
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO MetricLocalizations (EntityId, LanguageId, Name, Value, TranslationStatus, CreatedAt) VALUES ({0}, {1}, {2}, {3}, {4}, SYSDATETIMEOFFSET())",
                ids[i],
                languageId,
                $"Translated metric {i}",
                $"{1000 + (i * 37)} translated",
                i % 3);
        }
    }

    // Every metric and metric localization column that exists on both sides of the migration.
    private async Task<string[]> SnapshotAsync()
    {
        await using var context = Context();
        var metrics = await context.Database.SqlQueryRaw<string>(
            """
            SELECT CONCAT_WS('|', Id, StatisticId, Name, Value, Type, COALESCE(CAST(Prefix AS nvarchar(10)), N'null'),
                Priority, IsHidden, IsAutoSynced, CONVERT(nvarchar(40), CreatedAt, 127)) AS Value
            FROM Metrics
            """).ToListAsync();
        var localizations = await context.Database.SqlQueryRaw<string>(
            """
            SELECT CONCAT_WS('|', EntityId, LanguageId, Name, COALESCE(Value, N'null'), TranslationStatus,
                CONVERT(nvarchar(40), CreatedAt, 127)) AS Value
            FROM MetricLocalizations
            """).ToListAsync();

        return [.. metrics.Order(StringComparer.Ordinal), .. localizations.Order(StringComparer.Ordinal).Select(l => "loc|" + l)];
    }

    private static async Task<long> InsertAsync(VictoryCenterDbContext context, string sql, params object[] parameters)
    {
        var rows = await context.Database.SqlQueryRaw<long>(sql, parameters).ToListAsync();
        return rows.Single();
    }

    private async Task<long> GetStatisticIdAsync(long metricId)
    {
        await using var context = Context();
        return (await context.Database
            .SqlQueryRaw<long>("SELECT StatisticId AS Value FROM Metrics WHERE Id = {0}", metricId)
            .ToListAsync()).Single();
    }

    private async Task<int[]> GetValuesAsync(string column, long[] ids)
    {
        var sql = column switch
        {
            "Kind" => "SELECT Kind AS Value FROM Metrics WHERE Id = {0}",
            "Type" => "SELECT Type AS Value FROM Metrics WHERE Id = {0}",
            _ => throw new ArgumentOutOfRangeException(nameof(column), column, null)
        };

        await using var context = Context();
        var values = new int[ids.Length];
        for (var i = 0; i < ids.Length; i++)
        {
            values[i] = (await context.Database.SqlQueryRaw<int>(sql, ids[i]).ToListAsync()).Single();
        }

        return values;
    }

    private async Task<bool> KindColumnExistsAsync()
    {
        await using var context = Context();
        var rows = await context.Database
            .SqlQueryRaw<int>("SELECT CASE WHEN COL_LENGTH(N'Metrics', N'Kind') IS NULL THEN 0 ELSE 1 END AS Value")
            .ToListAsync();
        return rows.Single() == 1;
    }

    // The Metric mapping of the previous backend version, which only knows the Type column.
    private sealed class LegacyMetric
    {
        public long Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long StatisticId { get; set; }

        public int Value { get; set; }

        public string Name { get; set; } = null!;

        public LegacyMetricType Type { get; set; }

        public MetricPrefix? Prefix { get; set; }

        public bool IsAutoSynced { get; set; }

        public bool IsHidden { get; set; }

        public long Priority { get; set; }

        public byte[] RowVersion { get; set; } = [];
    }

    private sealed class LegacyMetricsContext(DbContextOptions<LegacyMetricsContext> options) : DbContext(options)
    {
        public DbSet<LegacyMetric> Metrics => Set<LegacyMetric>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var metric = modelBuilder.Entity<LegacyMetric>();
            metric.ToTable("Metrics");
            metric.Property(m => m.IsAutoSynced).HasDefaultValue(false);
            metric.Property(m => m.IsHidden).HasDefaultValue(false);
            metric.Property(m => m.RowVersion).IsRowVersion();
        }
    }
}
