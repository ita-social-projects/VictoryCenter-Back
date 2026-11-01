using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Migrations;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageLifecycleTests
{
    [Theory]
    [InlineData(StorageFileKind.Image, "upload.png")]
    [InlineData(StorageFileKind.Pdf, "upload.pdf")]
    public async Task MetadataCommit_CompletesReservationForBothFileKinds(StorageFileKind kind, string name)
    {
        await using var context = CreateContext();
        var operation = Operation(name, StorageOperationKind.UploadCleanup);
        operation.FileKind = kind;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        if (kind == StorageFileKind.Image)
        {
            context.Images.Add(NewImage("upload"));
        }
        else
        {
            context.PdfReports.Add(new PdfReport { Name = "Test report", BlobName = name });
        }

        await context.SaveChangesAsync();

        Assert.Equal(StorageOperationStatus.Completed, operation.Status);
        Assert.NotNull(operation.CompletedAt);
        Assert.Single(await context.StorageOperations.ToListAsync());
    }

    [Fact]
    public async Task BulkMetadataDelete_QueuesEveryImageWithoutDeletingFiles()
    {
        await using var context = CreateContext();
        var images = new[] { NewImage("first"), NewImage("second") };
        context.Images.AddRange(images);
        await context.SaveChangesAsync();
        foreach (var image in images)
        {
            await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, image.BlobName + ".png"), "keep until worker");
        }

        context.Images.RemoveRange(images);
        await context.SaveChangesAsync();

        Assert.Equal(new[] { "first.png", "second.png" }, await context.StorageOperations.OrderBy(operation => operation.FileName)
            .Select(operation => operation.FileName).ToArrayAsync());
        Assert.All(images, image => Assert.True(File.Exists(Files().GetPath(StorageFileKind.Image, image.BlobName + ".png"))));
    }

    [Fact]
    public async Task Reservation_ExistingFileIsNeverOverwrittenOrReserved()
    {
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();
        var path = Files().GetPath(StorageFileKind.Image, "existing.png");
        await File.WriteAllTextAsync(path, "original bytes");

        await Assert.ThrowsAsync<IOException>(() => Reservations(provider).ReserveAsync(StorageFileKind.Image, "existing.png"));

        Assert.Equal("original bytes", await File.ReadAllTextAsync(path));
        await using var verification = CreateContext();
        Assert.Empty(await verification.StorageOperations.ToListAsync());
    }

    [Theory]
    [InlineData(StorageOperationStatus.Completed)]
    [InlineData(StorageOperationStatus.Failed)]
    [InlineData(StorageOperationStatus.Processing)]
    public async Task Reservation_TerminalOrProcessingIdentifierCannotBeReused(StorageOperationStatus status)
    {
        await using var context = CreateContext();
        var operation = Operation("reserved.png", StorageOperationKind.UploadCleanup);
        operation.Status = status;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();

        await Assert.ThrowsAsync<IOException>(() => Reservations(provider).ReserveAsync(StorageFileKind.Image, "reserved.png"));

        Assert.Single(await context.StorageOperations.ToListAsync());
    }

    [Theory]
    [InlineData(StorageFileKind.Image, "delete.png")]
    [InlineData(StorageFileKind.Pdf, "delete.pdf")]
    public async Task DirectDeletion_IsPersistedAndExecutedOnlyByProcessor(StorageFileKind kind, string name)
    {
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();
        var path = Files().GetPath(kind, name);
        await File.WriteAllTextAsync(path, "original bytes");

        Reservations(provider).ScheduleDeletion(kind, name);

        Assert.True(File.Exists(path));
        await using var context = CreateContext();
        Assert.Equal(StorageOperationKind.Delete, (await context.StorageOperations.SingleAsync()).Kind);
        Assert.Equal(1, await Processor(context).ProcessBatchAsync(CancellationToken.None));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task Processor_RespectsBatchSizeDueOrderAndTerminalStates()
    {
        await using var context = CreateContext();
        _options.BatchSize = 1;
        var first = Operation("first.png");
        first.NextAttemptAt = _now.AddMinutes(-2);
        var second = Operation("second.png");
        var future = Operation("future.png");
        future.NextAttemptAt = _now.AddHours(1);
        var completed = Operation("completed.png");
        completed.Status = StorageOperationStatus.Completed;
        var failed = Operation("failed.png");
        failed.Status = StorageOperationStatus.Failed;
        context.StorageOperations.AddRange(first, second, future, completed, failed);
        await context.SaveChangesAsync();

        Assert.Equal(1, await Processor(context).ProcessBatchAsync(CancellationToken.None));

        var states = await context.StorageOperations.AsNoTracking().ToDictionaryAsync(operation => operation.Id);
        Assert.Equal(StorageOperationStatus.Completed, states[first.Id].Status);
        Assert.Equal(StorageOperationStatus.Pending, states[second.Id].Status);
        Assert.Equal(0, states[future.Id].Attempts);
        Assert.Equal(0, states[completed.Id].Attempts);
        Assert.Equal(0, states[failed.Id].Attempts);
    }

    [Fact]
    public async Task Processor_ExhaustedLeaseFailsWithoutTouchingFile()
    {
        await using var context = CreateContext();
        var operation = Operation("exhausted.png");
        operation.Status = StorageOperationStatus.Processing;
        operation.Attempts = _options.MaxAttempts;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        var path = Files().GetPath(StorageFileKind.Image, operation.FileName);
        await File.WriteAllTextAsync(path, "not deleted");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Failed, saved.Status);
        Assert.Equal("RetryLimitReached", saved.LastErrorCode);
        Assert.Equal(_options.MaxAttempts, saved.Attempts);
        Assert.Equal("not deleted", await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task Processor_ReferencedUploadIsAcknowledgedWithoutDeletion()
    {
        await using var context = CreateContext();
        context.Images.Add(NewImage("live"));
        await context.SaveChangesAsync();
        context.StorageOperations.Add(Operation("live.png", StorageOperationKind.UploadCleanup));
        await context.SaveChangesAsync();
        var path = Files().GetPath(StorageFileKind.Image, "live.png");
        await File.WriteAllTextAsync(path, "live bytes");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Completed, saved.Status);
        Assert.Null(saved.LastErrorCode);
        Assert.Equal("live bytes", await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task Processor_CancellationBeforeClaimLeavesOperationPending()
    {
        await using var context = CreateContext();
        context.StorageOperations.Add(Operation("pending.png"));
        await context.SaveChangesAsync();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Processor(context).ProcessBatchAsync(cancellation.Token));

        var operation = await context.StorageOperations.AsNoTracking().SingleAsync();
        Assert.Equal(StorageOperationStatus.Pending, operation.Status);
        Assert.Equal(0, operation.Attempts);
    }

    [Fact]
    public async Task Processor_CancellationAfterClaimLeavesRecoverableLease()
    {
        await using (var seed = CreateContext())
        {
            seed.StorageOperations.Add(Operation("cancelled.png"));
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, "cancelled.png");
        await File.WriteAllTextAsync(path, "keep until recovery");
        using var cancellation = new CancellationTokenSource();
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions)
            .AddInterceptors(new StorageSaveInterceptor(after: _ => cancellation.Cancel())).Options;
        await using (var interrupted = new VictoryCenterDbContext(options))
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Processor(interrupted).ProcessBatchAsync(cancellation.Token));
        }

        await using var recovery = CreateContext();
        var operation = await recovery.StorageOperations.AsNoTracking().SingleAsync();
        Assert.Equal(StorageOperationStatus.Processing, operation.Status);
        Assert.Equal(_now.AddSeconds(_options.LeaseSeconds), operation.NextAttemptAt);
        Assert.True(File.Exists(path));
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddSeconds(_options.LeaseSeconds + 1));
        Assert.Equal(1, await Processor(recovery).ProcessBatchAsync(CancellationToken.None));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task Processor_FailedClaimDoesNotDeleteFileOrConsumePersistedAttempt()
    {
        await using (var seed = CreateContext())
        {
            seed.StorageOperations.Add(Operation("unclaimed.png"));
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, "unclaimed.png");
        await File.WriteAllTextAsync(path, "keep");
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions)
            .AddInterceptors(new StorageSaveInterceptor(before: _ => throw new DbUpdateException("Simulated claim failure"))).Options;
        await using (var failed = new VictoryCenterDbContext(options))
        {
            await Assert.ThrowsAsync<DbUpdateException>(() => Processor(failed).ProcessBatchAsync(CancellationToken.None));
        }

        await using var verification = CreateContext();
        var saved = await verification.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Pending, saved.Status);
        Assert.Equal(0, saved.Attempts);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Processor_LostClaimSkipsFilesystemWork()
    {
        await using (var seed = CreateContext())
        {
            seed.StorageOperations.Add(Operation("contended.png"));
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, "contended.png");
        await File.WriteAllTextAsync(path, "keep");
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions)
            .AddInterceptors(new StorageSaveInterceptor(before: _ =>
            {
                using var winner = CreateContext();
                var operation = winner.StorageOperations.Single();
                operation.Status = StorageOperationStatus.Processing;
                operation.NextAttemptAt = _now.AddMinutes(5);
                operation.Version = Guid.NewGuid();
                winner.SaveChanges();
            })).Options;
        await using var loser = new VictoryCenterDbContext(options);

        Assert.Equal(0, await Processor(loser).ProcessBatchAsync(CancellationToken.None));
        Assert.Equal("keep", await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task Processor_LostAcknowledgementRemainsRecoverableAfterLease()
    {
        await using (var seed = CreateContext())
        {
            seed.StorageOperations.Add(Operation("ack-race.png"));
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, "ack-race.png");
        await File.WriteAllTextAsync(path, "delete once");
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions)
            .AddInterceptors(new StorageSaveInterceptor(before: context =>
            {
                if (context.ChangeTracker.Entries<StorageOperation>().Any(entry => entry.Entity.Status == StorageOperationStatus.Completed))
                {
                    using var winner = CreateContext();
                    var operation = winner.StorageOperations.Single();
                    operation.Version = Guid.NewGuid();
                    winner.SaveChanges();
                }
            })).Options;
        await using (var loser = new VictoryCenterDbContext(options))
        {
            Assert.Equal(1, await Processor(loser).ProcessBatchAsync(CancellationToken.None));
        }

        Assert.False(File.Exists(path));
        await using var recovery = CreateContext();
        Assert.Equal(StorageOperationStatus.Processing, (await recovery.StorageOperations.AsNoTracking().SingleAsync()).Status);
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddSeconds(_options.LeaseSeconds + 1));
        Assert.Equal(1, await Processor(recovery).ProcessBatchAsync(CancellationToken.None));
        Assert.Equal(StorageOperationStatus.Completed, (await recovery.StorageOperations.SingleAsync()).Status);
    }

    [Fact]
    public async Task Processor_LogsIdentifiersAndCodesWithoutFileNameOrExceptionDetails()
    {
        await using var context = CreateContext();
        var operation = Operation("sensitive-name.png");
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        Directory.CreateDirectory(Files().GetPath(StorageFileKind.Image, operation.FileName));
        var logger = new Mock<ILogger<StorageOperationProcessor>>();
        var processor = new StorageOperationProcessor(context, Files(), Options.Create(_options), _clock.Object, logger.Object);

        await processor.ProcessBatchAsync(CancellationToken.None);

        var logs = logger.Invocations.Where(invocation => invocation.Method.Name == "Log").ToList();
        var text = string.Join('\n', logs.Select(invocation => invocation.Arguments[2].ToString()));
        Assert.Contains(operation.Id.ToString(), text);
        Assert.DoesNotContain("sensitive-name", text);
        Assert.All(logs, invocation => Assert.Null(invocation.Arguments[3]));
    }

    [Theory]
    [InlineData(StorageFileKind.Image, "missing.png")]
    [InlineData(StorageFileKind.Pdf, "missing.pdf")]
    public async Task Reconciliation_RecordsMissingFilesForBothKinds(StorageFileKind kind, string name)
    {
        await using var context = CreateContext();
        if (kind == StorageFileKind.Image)
        {
            context.Images.Add(NewImage("missing"));
        }
        else
        {
            context.PdfReports.Add(new PdfReport { Name = "Missing", BlobName = name });
        }

        await context.SaveChangesAsync();
        Assert.Equal(1, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Equal(1, await Reconciliation(context).ScanAsync(CancellationToken.None));
        var finding = await context.StorageReconciliationFindings.SingleAsync();
        Assert.Equal(StorageFindingKind.Missing, finding.Kind);
        Assert.Equal(name, finding.FileName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Reconciliation_ResolvesStaleFindingAfterOrphanOrMetadataDisappears(bool orphan)
    {
        await using var context = CreateContext();
        var path = Files().GetPath(StorageFileKind.Image, "stale.png");
        if (orphan)
        {
            await File.WriteAllTextAsync(path, "orphan");
        }
        else
        {
            context.Images.Add(NewImage("stale"));
            await context.SaveChangesAsync();
        }

        await Reconciliation(context).ScanAsync(CancellationToken.None);
        if (orphan)
        {
            File.Delete(path);
        }
        else
        {
            context.Images.Remove(await context.Images.SingleAsync());
            await context.SaveChangesAsync();
        }

        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddMinutes(1));
        await Reconciliation(context).ScanAsync(CancellationToken.None);

        Assert.NotNull((await context.StorageReconciliationFindings.SingleAsync()).ResolvedAt);
    }

    [Fact]
    public async Task Reconciliation_ReappearingFindingRestartsSafetyAge()
    {
        await using var context = CreateContext();
        var path = Files().GetPath(StorageFileKind.Image, "reappeared.png");
        await File.WriteAllTextAsync(path, "orphan");
        await Reconciliation(context).ScanAsync(CancellationToken.None);
        var original = await context.StorageReconciliationFindings.SingleAsync();
        original.CleanupOperationId = Guid.NewGuid();
        await context.SaveChangesAsync();
        File.Delete(path);
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddDays(2));
        await Reconciliation(context).ScanAsync(CancellationToken.None);
        await File.WriteAllTextAsync(path, "new orphan");
        var rediscoveredAt = _now.AddDays(3);
        _clock.Setup(clock => clock.GetUtcNow()).Returns(rediscoveredAt);

        await Reconciliation(context).ScanAsync(CancellationToken.None);

        var finding = await context.StorageReconciliationFindings.SingleAsync();
        Assert.Null(finding.ResolvedAt);
        Assert.Equal(rediscoveredAt, finding.FirstSeenAt);
        Assert.Null(finding.CleanupOperationId);
    }

    [Fact]
    public async Task Reconciliation_ScansBeyondOneDatabasePageWithoutDuplicates()
    {
        await using var context = CreateContext();
        const int uniqueFiles = 205;
        context.Images.AddRange(Enumerable.Range(0, uniqueFiles).Select(index => NewImage($"page-{index}")));
        context.Images.Add(NewImage("page-0"));
        await context.SaveChangesAsync();

        Assert.Equal(uniqueFiles, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Equal(uniqueFiles, await context.StorageReconciliationFindings.CountAsync());
    }

    [Theory]
    [InlineData(StorageOperationKind.UploadCleanup, StorageOperationStatus.Pending)]
    [InlineData(StorageOperationKind.UploadCleanup, StorageOperationStatus.Processing)]
    [InlineData(StorageOperationKind.Delete, StorageOperationStatus.Pending)]
    [InlineData(StorageOperationKind.Delete, StorageOperationStatus.Processing)]
    public async Task Reconciliation_ProtectsActiveOperationsAndLegacyTemporaryFiles(StorageOperationKind kind, StorageOperationStatus status)
    {
        await using var context = CreateContext();
        var operation = Operation("protected.png", kind);
        operation.Status = status;
        operation.NextAttemptAt = _now.AddHours(1);
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, "protected.png"), "pending bytes");
        if (kind == StorageOperationKind.UploadCleanup)
        {
            await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, $".protected.{Guid.NewGuid():N}.tmp"), "legacy upload bytes");
        }

        Assert.Equal(0, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Fact]
    public async Task Reconciliation_UnavailableRootRaisesErrorInsteadOfReportingAnEmptyInventory()
    {
        await using var context = CreateContext();
        var files = Files();
        Directory.Delete(Path.Combine(_root, "images"));
        var reconciliation = new StorageReconciliation(context, files, _clock.Object);

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => reconciliation.ScanAsync(CancellationToken.None));
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Theory]
    [InlineData("mismatched-file")]
    [InlineData("mismatched-kind")]
    [InlineData("missing-kind")]
    [InlineData("resolved")]
    [InlineData("recent-finding")]
    [InlineData("recent-file")]
    [InlineData("referenced")]
    [InlineData("active")]
    public async Task Cleanup_ProcessorRejectsUnsafeRequestWithoutDeletingFile(string condition)
    {
        await using var context = CreateContext();
        var path = Files().GetPath(StorageFileKind.Image, "unsafe.png");
        await File.WriteAllTextAsync(path, "keep");
        File.SetLastWriteTimeUtc(path, _now.AddDays(-3).UtcDateTime);
        var operation = Operation("unsafe.png");
        operation.RequestedBy = "admin";
        context.StorageOperations.Add(operation);
        var finding = new StorageReconciliationFinding
        {
            FileKind = condition == "mismatched-kind" ? StorageFileKind.Pdf : StorageFileKind.Image,
            FileName = condition == "mismatched-file" ? "different.png" : "unsafe.png",
            Kind = condition == "missing-kind" ? StorageFindingKind.Missing : StorageFindingKind.Unreferenced,
            FirstSeenAt = condition == "recent-finding" ? _now : _now.AddDays(-3),
            LastSeenAt = _now,
            ResolvedAt = condition == "resolved" ? _now : null,
            CleanupOperationId = operation.Id
        };
        context.StorageReconciliationFindings.Add(finding);
        if (condition == "referenced")
        {
            context.Images.Add(NewImage("unsafe"));
        }

        if (condition == "active")
        {
            var active = Operation("unsafe.png", StorageOperationKind.UploadCleanup);
            active.NextAttemptAt = _now.AddHours(1);
            context.StorageOperations.Add(active);
        }

        if (condition == "recent-file")
        {
            File.SetLastWriteTimeUtc(path, _now.UtcDateTime);
        }

        await context.SaveChangesAsync();

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        var saved = await context.StorageOperations.SingleAsync(item => item.Id == operation.Id);
        Assert.Equal(StorageOperationStatus.Failed, saved.Status);
        Assert.Equal(condition == "referenced" ? "FileStillReferenced" : "CleanupNotSafe", saved.LastErrorCode);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Cleanup_AlreadyAbsentOrphanIsIdempotentlyAcknowledged()
    {
        await using var context = CreateContext();
        var operation = Operation("absent.png");
        operation.RequestedBy = "admin";
        context.StorageOperations.Add(operation);
        context.StorageReconciliationFindings.Add(new StorageReconciliationFinding
        {
            FileKind = StorageFileKind.Image, FileName = operation.FileName, Kind = StorageFindingKind.Unreferenced,
            FirstSeenAt = _now.AddDays(-3), LastSeenAt = _now, CleanupOperationId = operation.Id
        });
        await context.SaveChangesAsync();

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal(StorageOperationStatus.Completed, (await context.StorageOperations.SingleAsync()).Status);
    }

    [Fact]
    public async Task Reconciliation_IgnoresUnmanagedAndNestedFiles()
    {
        await using var context = CreateContext();
        var root = Path.Combine(_root, "images");
        await File.WriteAllTextAsync(Path.Combine(root, "notes.txt"), "not managed");
        await File.WriteAllTextAsync(Path.Combine(root, ".invalid.tmp"), "not managed");
        Directory.CreateDirectory(Path.Combine(root, "nested"));
        await File.WriteAllTextAsync(Path.Combine(root, "nested", "nested.png"), "not managed");

        Assert.Equal(0, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(".")]
    public void Paths_RejectEmptyOrSpecialNames(string? name)
    {
        Assert.Throws<ArgumentException>(() => Files().GetPath(StorageFileKind.Image, name!));
    }

    [Fact]
    public void Paths_RejectExcessiveLengthAndUnknownFileKind()
    {
        using var context = CreateContext();
        Assert.Throws<ArgumentException>(() => Files().GetPath(StorageFileKind.Image, new string('a', StorageFileNames.MaxLength + 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => Files().GetPath((StorageFileKind)int.MaxValue, "file.png"));
        Assert.Throws<ArgumentOutOfRangeException>(() => StorageReferences.Inventory(context, (StorageFileKind)int.MaxValue));
    }

    [Fact]
    public void Exists_RejectsDirectoryInsteadOfSilentlyTreatingItAsMissing()
    {
        var path = Files().GetPath(StorageFileKind.Image, "directory.png");
        Directory.CreateDirectory(path);
        Assert.Throws<IOException>(() => Files().Exists(StorageFileKind.Image, "directory.png"));
    }

    [UnixFileSystemFact]
    public async Task Processor_RejectsSymbolicLinkWithoutDeletingItsTarget()
    {
        var target = Path.Combine(_root, "outside-managed-root.png");
        await File.WriteAllTextAsync(target, "outside target bytes");
        var link = Files().GetPath(StorageFileKind.Image, "link.png");
        File.CreateSymbolicLink(link, target);
        await using var context = CreateContext();
        context.StorageOperations.Add(Operation("link.png"));
        await context.SaveChangesAsync();

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal("outside target bytes", await File.ReadAllTextAsync(target));
        Assert.Equal(StorageOperationStatus.Pending, (await context.StorageOperations.SingleAsync()).Status);
        Assert.True(File.Exists(link));
    }

    [Fact]
    public void Migration_DefinesRecoveryTablesUniqueReservationAndConcurrencyTokens()
    {
        var migration = new AddStorageLifecycle();
        Assert.Equal(2, migration.UpOperations.OfType<CreateTableOperation>().Count());
        Assert.Contains(migration.UpOperations.OfType<AddColumnOperation>(), column => column.Name == nameof(Image.StorageVersion));
        Assert.Contains(migration.UpOperations.OfType<CreateIndexOperation>(), index => index.IsUnique
            && index.Table == "StorageOperations" && index.Filter == "[Kind] = 0");
        Assert.Equal(2, migration.DownOperations.OfType<DropTableOperation>().Count());
        using var context = CreateContext();
        Assert.True(context.Model.FindEntityType(typeof(Image))!.FindProperty(nameof(Image.StorageVersion))!.IsConcurrencyToken);
        Assert.True(context.Model.FindEntityType(typeof(StorageOperation))!.FindProperty(nameof(StorageOperation.Version))!.IsConcurrencyToken);
        Assert.True(context.Model.FindEntityType(typeof(StorageReconciliationFinding))!
            .FindProperty(nameof(StorageReconciliationFinding.CleanupOperationId))!.IsConcurrencyToken);
    }

    [Theory]
    [InlineData(nameof(StorageLifecycleOptions.BatchSize), 0)]
    [InlineData(nameof(StorageLifecycleOptions.MaxAttempts), 0)]
    [InlineData(nameof(StorageLifecycleOptions.LeaseSeconds), 1)]
    [InlineData(nameof(StorageLifecycleOptions.PollIntervalSeconds), 0)]
    [InlineData(nameof(StorageLifecycleOptions.UploadReservationHours), 0)]
    [InlineData(nameof(StorageLifecycleOptions.OrphanMinimumAgeHours), 0)]
    [InlineData(nameof(StorageLifecycleOptions.ReconciliationIntervalMinutes), 0)]
    [InlineData(nameof(StorageLifecycleOptions.RetryDelaySeconds), 0)]
    [InlineData(nameof(StorageLifecycleOptions.MaxRetryDelaySeconds), 0)]
    public void Configuration_RejectsInvalidOperationalLimits(string property, int value)
    {
        typeof(StorageLifecycleOptions).GetProperty(property)!.SetValue(_options, value);
        Assert.False(Validator.TryValidateObject(_options, new ValidationContext(_options), [], validateAllProperties: true));
    }

    [Theory]
    [InlineData(5, 10, true)]
    [InlineData(10, 5, false)]
    [InlineData(0, 5, false)]
    public void Configuration_StartupValidationEnforcesBoundAndCrossFieldRules(int initial, int maximum, bool valid)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"StorageLifecycle:{nameof(StorageLifecycleOptions.RetryDelaySeconds)}"] = initial.ToString(CultureInfo.InvariantCulture),
            [$"StorageLifecycle:{nameof(StorageLifecycleOptions.MaxRetryDelaySeconds)}"] = maximum.ToString(CultureInfo.InvariantCulture)
        }).Build();
        var services = new ServiceCollection();
        services.AddApplicationServices(configuration);
        using var provider = services.BuildServiceProvider();
        var validator = provider.GetRequiredService<IStartupValidator>();

        if (valid)
        {
            validator.Validate();
        }
        else
        {
            Assert.Throws<OptionsValidationException>(() => validator.Validate());
        }
    }

    private sealed class StorageSaveInterceptor(Action<DbContext>? before = null, Action<DbContext>? after = null) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            before?.Invoke(eventData.Context!);
            return ValueTask.FromResult(result);
        }

        public override ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            after?.Invoke(eventData.Context!);
            return ValueTask.FromResult(result);
        }
    }

    private sealed class UnixFileSystemFactAttribute : FactAttribute
    {
        public UnixFileSystemFactAttribute()
        {
            if (OperatingSystem.IsWindows())
            {
                Skip = "Creating Windows symbolic links requires privileges; this filesystem case runs on Linux CI.";
            }
        }
    }
}
