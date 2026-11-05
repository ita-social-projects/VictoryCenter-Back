using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.StorageLifecycle;

public partial class StorageSqlServerTests
{
    [StorageSqlServerFact]
    public async Task Reconciliation_UsesBoundedSqlCommandsForMultipleFileBatches()
    {
        const int fileCount = 205;
        for (var index = 0; index < fileCount; index++)
        {
            await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, $"orphan-{index}.png"), "orphan content");
        }

        var counter = new ReconciliationCommandCounter();
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions).AddInterceptors(counter).Options;
        await using var context = new VictoryCenterDbContext(options, _clock.Object);
        var reconciliation = new StorageReconciliation(context, Files(), _clock.Object, NullLogger<StorageReconciliation>.Instance);

        Assert.Equal(fileCount, await reconciliation.ScanAsync(CancellationToken.None));

        // Includes EF's provider-sized INSERT batches, not just the SELECT queries.
        Assert.InRange(counter.Commands, 1, 30);
        Assert.Equal(fileCount, await context.StorageReconciliationFindings.CountAsync());
    }

    [StorageSqlServerFact]
    public async Task Reconciliation_ConcurrentNewFindingBatchesRecoverFromUniqueConstraintRace()
    {
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, "shared-orphan.png"), "orphan content");
        var barrier = new SaveBarrier(database => database.ChangeTracker.Entries<StorageReconciliationFinding>()
            .Any(entry => entry.State == EntityState.Added));
        await using var first = Context(barrier);
        await using var second = Context(barrier);
        var firstScan = new StorageReconciliation(first, Files(), _clock.Object, NullLogger<StorageReconciliation>.Instance);
        var secondScan = new StorageReconciliation(second, Files(), _clock.Object, NullLogger<StorageReconciliation>.Instance);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var findings = await Task.WhenAll(firstScan.ScanAsync(timeout.Token), secondScan.ScanAsync(timeout.Token));

        Assert.All(findings, count => Assert.Equal(1, count));
        await using var verification = Context();
        Assert.Equal("shared-orphan.png", (await verification.StorageReconciliationFindings.SingleAsync()).FileName);
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Image, "shared-orphan.png")));
    }

    private sealed class ReconciliationCommandCounter : DbCommandInterceptor
    {
        public int Commands { get; private set; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Commands++;
            return ValueTask.FromResult(result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Commands++;
            return ValueTask.FromResult(result);
        }
    }
}
