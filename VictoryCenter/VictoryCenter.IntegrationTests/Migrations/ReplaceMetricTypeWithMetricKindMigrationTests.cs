using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.StorageLifecycle;

namespace VictoryCenter.IntegrationTests.Migrations;

public class ReplaceMetricTypeWithMetricKindMigrationTests : IAsyncLifetime
{
    private const string DatabasePrefix = "VictoryCenter_MetricKindMigrationTests_";
    private const string PreviousMigration = "_AddAuditLog";
    private const string TargetMigration = "_ReplaceMetricTypeWithMetricKind";

    // Old MetricType values.
    private const int Partners = 0;
    private const int Programs = 1;
    private const int Raised = 2;
    private const int TherapyHours = 3;

    private readonly string _databaseName = DatabasePrefix + Guid.NewGuid().ToString("N");
    private DbContextOptions<VictoryCenterDbContext> _databaseOptions = null!;
    private string _previous = null!;
    private string _target = null!;

    public async Task InitializeAsync()
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(StorageSqlServerFactAttribute.ConnectionVariable))
        {
            InitialCatalog = _databaseName,
            ConnectTimeout = 10
        };
        _databaseOptions = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer(connection.ConnectionString, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "entity_framework"))
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
    public async Task Up_MapsRaisedToRaisedFundsAndOtherTypesToStandard()
    {
        var ids = await SeedStatisticAsync(TherapyHours, Raised, Partners, Programs);

        await MigrateAsync(_target);

        var kinds = await GetValuesAsync("Kind", ids);
        Assert.Equal(new[] { 0, 1, 0, 0 }, kinds);
    }

    [StorageSqlServerFact]
    public async Task Down_RestoresOriginalTypesRegardlessOfPriority()
    {
        // Seeded out of the Partners/Programs/TherapyHours order, so a priority-based guess would be wrong.
        var ids = await SeedStatisticAsync(TherapyHours, Raised, Partners, Programs);
        await MigrateAsync(_target);

        await MigrateAsync(_previous);

        var types = await GetValuesAsync("Type", ids);
        Assert.Equal(new[] { TherapyHours, Raised, Partners, Programs }, types);
        Assert.False(await BackupTableExistsAsync());
    }

    [StorageSqlServerFact]
    public async Task Down_FailsWithoutChangesWhenStandardMetricHasNoBackup()
    {
        var ids = await SeedStatisticAsync(Partners, Programs, TherapyHours, Raised);
        await MigrateAsync(_target);
        await using (var context = Context())
        {
            await context.Database.ExecuteSqlRawAsync("DELETE FROM MetricTypeBackup WHERE MetricId = {0}", ids[0]);
        }

        var exception = await Assert.ThrowsAsync<SqlException>(() => MigrateAsync(_previous));

        Assert.Contains("MetricTypeBackup", exception.Message);
        var kinds = await GetValuesAsync("Kind", ids);
        Assert.Equal(new[] { 0, 0, 0, 1 }, kinds);
        await using (var context = Context())
        {
            Assert.Contains(_target, await context.Database.GetAppliedMigrationsAsync());
        }
    }

    [StorageSqlServerFact]
    public async Task Down_SucceedsWithoutBackupTableWhenThereAreNoStandardMetrics()
    {
        await MigrateAsync(_target);
        await using (var context = Context())
        {
            await context.Database.ExecuteSqlRawAsync("DROP TABLE MetricTypeBackup");
        }

        await MigrateAsync(_previous);

        await using (var context = Context())
        {
            Assert.DoesNotContain(_target, await context.Database.GetAppliedMigrationsAsync());
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

    private static async Task<long> InsertAsync(VictoryCenterDbContext context, string sql, params object[] parameters)
    {
        var rows = await context.Database.SqlQueryRaw<long>(sql, parameters).ToListAsync();
        return rows.Single();
    }

    private async Task<int[]> GetValuesAsync(string column, long[] ids)
    {
        // The column was renamed by the migration, so the query depends on which side of it the database is on.
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

    private async Task<bool> BackupTableExistsAsync()
    {
        await using var context = Context();
        var rows = await context.Database
            .SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID(N'MetricTypeBackup', N'U') IS NULL THEN 0 ELSE 1 END AS Value")
            .ToListAsync();
        return rows.Single() == 1;
    }
}
