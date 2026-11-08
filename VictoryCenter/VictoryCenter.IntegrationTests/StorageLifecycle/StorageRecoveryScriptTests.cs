using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.StorageLifecycle;

public partial class StorageSqlServerTests
{
    private static readonly string[] CleanupActors = ["first-admin", "second-admin"];
    [StorageSqlServerFact]
    public async Task QueueSummary_GroupsAndOrdersOperationsWithoutChangingData()
    {
        await using var context = Context();
        await context.Database.OpenConnectionAsync();

        async Task<List<(int FileKind, int Kind, int Status, long Count)>> ReadSummaryAsync()
        {
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, "StorageScripts", "summarize-operations.sql"));
            await using var reader = await command.ExecuteReaderAsync();
            List<(int FileKind, int Kind, int Status, long Count)> rows = [];
            while (await reader.ReadAsync())
            {
                rows.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt64(3)));
            }

            return rows;
        }

        Assert.Empty(await ReadSummaryAsync());
        var inputs = new[]
        {
            (StorageFileKind.Image, StorageOperationKind.Delete, StorageOperationStatus.Failed),
            (StorageFileKind.Image, StorageOperationKind.UploadCleanup, StorageOperationStatus.Pending),
            (StorageFileKind.Pdf, StorageOperationKind.Delete, StorageOperationStatus.Completed),
            (StorageFileKind.Pdf, StorageOperationKind.Delete, StorageOperationStatus.Processing),
            (StorageFileKind.Image, StorageOperationKind.Delete, StorageOperationStatus.Failed),
            (StorageFileKind.Image, StorageOperationKind.Delete, StorageOperationStatus.Pending)
        };
        context.StorageOperations.AddRange(inputs.Select((input, index) => new StorageOperation
        {
            FileKind = input.Item1,
            Kind = input.Item2,
            Status = input.Item3,
            FileName = $"summary-{index}.{(input.Item1 == StorageFileKind.Image ? "png" : "pdf")}",
            NextAttemptAt = _now
        }));
        await context.SaveChangesAsync();
        var snapshot = context.StorageOperations.AsNoTracking().OrderBy(operation => operation.Id)
            .Select(operation => new { operation.Id, operation.Status, operation.Attempts, operation.Version });
        var before = await snapshot.ToListAsync();

        Assert.Equal(
            new[]
            {
                ((int)StorageFileKind.Image, (int)StorageOperationKind.UploadCleanup, (int)StorageOperationStatus.Pending, 1L),
                ((int)StorageFileKind.Image, (int)StorageOperationKind.Delete, (int)StorageOperationStatus.Pending, 1L),
                ((int)StorageFileKind.Image, (int)StorageOperationKind.Delete, (int)StorageOperationStatus.Failed, 2L),
                ((int)StorageFileKind.Pdf, (int)StorageOperationKind.Delete, (int)StorageOperationStatus.Processing, 1L),
                ((int)StorageFileKind.Pdf, (int)StorageOperationKind.Delete, (int)StorageOperationStatus.Completed, 1L)
            },
            await ReadSummaryAsync());
        Assert.Equal(inputs.Length, await context.StorageOperations.CountAsync());
        Assert.Equal(before, await snapshot.ToListAsync());
    }

    [StorageSqlServerFact]
    public async Task CleanupScript_ConcurrentRequestsReuseOneAuditedOperation()
    {
        var finding = new StorageReconciliationFinding
        {
            FileKind = StorageFileKind.Image, FileName = "orphan.png", Kind = StorageFindingKind.Unreferenced,
            FirstSeenAt = _now.AddDays(-3), LastSeenAt = _now
        };
        await using (var seed = Context())
        {
            seed.StorageReconciliationFindings.Add(finding);
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, finding.FileName);
        await File.WriteAllTextAsync(path, "orphan bytes");
        File.SetLastWriteTimeUtc(path, _now.AddDays(-3).UtcDateTime);
        await using var first = Context();
        await using var second = Context();
        await Task.WhenAll(
            RunScriptAsync(first, "queue-orphan-cleanup.sql", "@FindingId", finding.Id, "first-admin"),
            RunScriptAsync(second, "queue-orphan-cleanup.sql", "@FindingId", finding.Id, "second-admin"));
        await using var verification = Context();
        var operation = await verification.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationKind.OrphanCleanup, operation.Kind);
        Assert.Contains(operation.RequestedBy, CleanupActors);
        Assert.Equal(operation.Id, (await verification.StorageReconciliationFindings.SingleAsync()).CleanupOperationId);
        Assert.True(File.Exists(path));
    }

    [StorageSqlServerFact]
    public async Task CleanupScript_RetryAfterFindingReappearsRetainsSafetyChecks()
    {
        await using var context = Context();
        var finding = new StorageReconciliationFinding
        {
            FileKind = StorageFileKind.Image,
            FileName = "rediscovered.png",
            Kind = StorageFindingKind.Unreferenced,
            FirstSeenAt = _now.AddDays(-3),
            LastSeenAt = _now.AddDays(-2)
        };
        context.StorageReconciliationFindings.Add(finding);
        await context.SaveChangesAsync();
        await RunScriptAsync(context, "queue-orphan-cleanup.sql", "@FindingId", finding.Id, "original-admin");
        context.ChangeTracker.Clear();
        var operation = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationKind.OrphanCleanup, operation.Kind);
        operation.Status = StorageOperationStatus.Failed;
        await context.SaveChangesAsync();
        var reconciliation = new StorageReconciliation(
            context, Files(), _clock.Object, NullLogger<StorageReconciliation>.Instance);
        await reconciliation.ScanAsync(CancellationToken.None);
        Assert.NotNull((await context.StorageReconciliationFindings.SingleAsync()).ResolvedAt);
        var path = Files().GetPath(StorageFileKind.Image, finding.FileName);
        await File.WriteAllTextAsync(path, "new orphan content");
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddMinutes(1));
        await reconciliation.ScanAsync(CancellationToken.None);
        Assert.Null((await context.StorageReconciliationFindings.SingleAsync()).CleanupOperationId);

        await RunScriptAsync(context, "retry-failed-operation.sql", "@OperationId", operation.Id, "retry-admin");
        var reset = await context.StorageOperations.AsNoTracking().SingleAsync();
        Assert.Equal(StorageOperationKind.OrphanCleanup, reset.Kind);
        _clock.Setup(clock => clock.GetUtcNow()).Returns(reset.NextAttemptAt.AddMinutes(1));
        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        var saved = await context.StorageOperations.AsNoTracking().SingleAsync();
        Assert.Equal(StorageOperationStatus.Failed, saved.Status);
        Assert.Equal("CleanupNotSafe", saved.LastErrorCode);
        Assert.Equal("new orphan content", await File.ReadAllTextAsync(path));
    }

    [StorageSqlServerFact]
    public async Task Reconciliation_OversizedFileDoesNotAbortSqlScan()
    {
        await using var context = Context();
        var root = Path.GetDirectoryName(Files().GetPath(StorageFileKind.Image, "normal.png"))!;
        var oversizedName = new string('a', StorageFileNames.MaxLength) + ".png";
        await File.WriteAllTextAsync(Path.Combine(root, oversizedName), "oversized orphan");
        await File.WriteAllTextAsync(Path.Combine(root, "normal.png"), "normal orphan");
        var reconciliation = new StorageReconciliation(
            context, Files(), _clock.Object, NullLogger<StorageReconciliation>.Instance);

        Assert.Equal(1, await reconciliation.ScanAsync(CancellationToken.None));
        Assert.Equal(1, await reconciliation.ScanAsync(CancellationToken.None));

        Assert.Equal("normal.png", (await context.StorageReconciliationFindings.SingleAsync()).FileName);
        Assert.True(File.Exists(Path.Combine(root, oversizedName)));
    }

    [StorageSqlServerFact]
    public async Task RetryScript_OnlyFailedOperationCanBeResetAndRecovered()
    {
        foreach (var status in Enum.GetValues<StorageOperationStatus>())
        {
            await using var context = Context();
            var operation = new StorageOperation
            {
                FileKind = StorageFileKind.Image, FileName = $"retry-{status}.png", Kind = StorageOperationKind.Delete,
                Status = status, Attempts = 8, NextAttemptAt = _now.AddHours(2)
            };
            context.StorageOperations.Add(operation);
            await context.SaveChangesAsync();
            if (status != StorageOperationStatus.Failed)
            {
                var error = await Assert.ThrowsAsync<SqlException>(() => RunScriptAsync(
                    context, "retry-failed-operation.sql", "@OperationId", operation.Id, "recovery-admin"));
                Assert.Equal(51002, error.Number);
                var unchanged = await context.StorageOperations.AsNoTracking().SingleAsync(item => item.Id == operation.Id);
                Assert.Equal(status, unchanged.Status);
                Assert.Equal(operation.Version, unchanged.Version);
                continue;
            }

            await RunScriptAsync(context, "retry-failed-operation.sql", "@OperationId", operation.Id, "recovery-admin");
            var reset = await context.StorageOperations.AsNoTracking().SingleAsync(item => item.Id == operation.Id);
            Assert.Equal(StorageOperationStatus.Pending, reset.Status);
            Assert.Equal(0, reset.Attempts);
            Assert.Equal("recovery-admin", reset.RequestedBy);
            Assert.NotEqual(operation.Version, reset.Version);
            _clock.Setup(clock => clock.GetUtcNow()).Returns(reset.NextAttemptAt.AddSeconds(1));
            await Processor(context).ProcessBatchAsync(CancellationToken.None);
            Assert.Equal(StorageOperationStatus.Completed, (await context.StorageOperations.SingleAsync(item => item.Id == operation.Id)).Status);
        }
    }

    [StorageSqlServerFact]
    public async Task CleanupScript_RejectsUnavailableFindingsAndMissingActor()
    {
        await using var context = Context();
        var unknown = await Assert.ThrowsAsync<SqlException>(() => RunScriptAsync(
            context, "queue-orphan-cleanup.sql", "@FindingId", Guid.NewGuid(), "admin"));
        Assert.Equal(51001, unknown.Number);
        var finding = new StorageReconciliationFinding
        {
            FileKind = StorageFileKind.Image, FileName = "missing.png", Kind = StorageFindingKind.Missing,
            FirstSeenAt = _now.AddDays(-3), LastSeenAt = _now
        };
        context.StorageReconciliationFindings.Add(finding);
        await context.SaveChangesAsync();
        var invalidKind = await Assert.ThrowsAsync<SqlException>(() => RunScriptAsync(
            context, "queue-orphan-cleanup.sql", "@FindingId", finding.Id, "admin"));
        Assert.Equal(51001, invalidKind.Number);
        finding.Kind = StorageFindingKind.Unreferenced;
        finding.ResolvedAt = _now;
        await context.SaveChangesAsync();
        var resolved = await Assert.ThrowsAsync<SqlException>(() => RunScriptAsync(
            context, "queue-orphan-cleanup.sql", "@FindingId", finding.Id, "admin"));
        Assert.Equal(51001, resolved.Number);
        var missingActor = await Assert.ThrowsAsync<SqlException>(() => RunScriptAsync(
            context, "queue-orphan-cleanup.sql", "@FindingId", finding.Id, " "));
        Assert.Equal(51000, missingActor.Number);
        Assert.Empty(await context.StorageOperations.ToListAsync());
    }

    private static async Task RunScriptAsync(VictoryCenterDbContext context, string script, string idParameter, Guid id, string actor)
    {
        var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "StorageScripts", script));
        await context.Database.ExecuteSqlRawAsync(
            sql,
            new SqlParameter(idParameter, SqlDbType.UniqueIdentifier) { Value = id },
            new SqlParameter("@Actor", SqlDbType.NVarChar, 200) { Value = actor });
    }
}
