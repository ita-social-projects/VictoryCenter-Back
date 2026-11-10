using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageLifecycleTests
{
    [Theory]
    [InlineData(StorageFileKind.Image, "unlinked.png", true)]
    [InlineData(StorageFileKind.Image, "unlinked.png", false)]
    [InlineData(StorageFileKind.Pdf, "unlinked.pdf", true)]
    [InlineData(StorageFileKind.Pdf, "unlinked.pdf", false)]
    public async Task Cleanup_MissingFindingFailsClosedRegardlessOfFilePresence(StorageFileKind kind, string name, bool exists)
    {
        await using var context = CreateContext();
        var operation = Operation(name, StorageOperationKind.OrphanCleanup);
        operation.FileKind = kind;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        var path = Files().GetPath(kind, name);
        if (exists)
        {
            await File.WriteAllTextAsync(path, "unreviewed content");
        }

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Failed, saved.Status);
        Assert.Equal("CleanupNotSafe", saved.LastErrorCode);
        Assert.Equal(exists, File.Exists(path));
        if (exists)
        {
            Assert.Equal("unreviewed content", await File.ReadAllTextAsync(path));
        }
    }

    [Theory]
    [InlineData(StorageFileKind.Image, "reappeared.png")]
    [InlineData(StorageFileKind.Pdf, "reappeared.pdf")]
    public async Task Cleanup_RetryAfterFindingReappearsCannotDeleteNewFile(StorageFileKind kind, string name)
    {
        await using var context = CreateContext();
        var operation = Operation(name, StorageOperationKind.OrphanCleanup);
        operation.FileKind = kind;
        operation.Status = StorageOperationStatus.Failed;
        operation.RequestedBy = "original-admin";
        context.StorageOperations.Add(operation);
        context.StorageReconciliationFindings.Add(new StorageReconciliationFinding
        {
            FileKind = kind,
            FileName = name,
            Kind = StorageFindingKind.Unreferenced,
            FirstSeenAt = _now.AddDays(-3),
            LastSeenAt = _now.AddDays(-2),
            ResolvedAt = _now.AddDays(-1),
            CleanupOperationId = operation.Id
        });
        await context.SaveChangesAsync();
        var path = Files().GetPath(kind, name);
        await File.WriteAllTextAsync(path, "new orphan content");

        await Reconciliation(context).ScanAsync(CancellationToken.None);

        var finding = await context.StorageReconciliationFindings.SingleAsync();
        Assert.Null(finding.CleanupOperationId);
        Assert.Equal(_now, finding.FirstSeenAt);
        operation.Status = StorageOperationStatus.Pending;
        operation.Attempts = 0;
        operation.RequestedBy = "retry-admin";
        operation.Version = Guid.NewGuid();
        await context.SaveChangesAsync();

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Failed, saved.Status);
        Assert.Equal("CleanupNotSafe", saved.LastErrorCode);
        Assert.Equal("new orphan content", await File.ReadAllTextAsync(path));
    }

    [Theory]
    [InlineData(StorageFileKind.Image, "retry.png")]
    [InlineData(StorageFileKind.Pdf, "retry.pdf")]
    public async Task Processor_OrdinaryDeletionWithRetryActorDoesNotRequireFinding(StorageFileKind kind, string name)
    {
        await using var context = CreateContext();
        var operation = Operation(name);
        operation.FileKind = kind;
        operation.RequestedBy = "retry-admin";
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        var path = Files().GetPath(kind, name);
        await File.WriteAllTextAsync(path, "obsolete content");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal(StorageOperationStatus.Completed, (await context.StorageOperations.SingleAsync()).Status);
        Assert.False(File.Exists(path));
    }

    [Theory]
    [InlineData(StorageFileKind.Image, ".png")]
    [InlineData(StorageFileKind.Pdf, ".pdf")]
    public async Task Cleanup_ReferencedOrphanIsNeverDeleted(StorageFileKind kind, string extension)
    {
        await using var context = CreateContext();
        var name = "referenced" + extension;
        var operation = Operation(name, StorageOperationKind.OrphanCleanup);
        operation.FileKind = kind;
        context.StorageOperations.Add(operation);
        if (kind == StorageFileKind.Image)
        {
            context.Images.Add(NewImage("referenced"));
        }
        else
        {
            context.PdfReports.Add(new PdfReport { Name = "Referenced", BlobName = name });
        }

        await context.SaveChangesAsync();
        var path = Files().GetPath(kind, name);
        await File.WriteAllTextAsync(path, "live content");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal("FileStillReferenced", (await context.StorageOperations.SingleAsync()).LastErrorCode);
        Assert.Equal("live content", await File.ReadAllTextAsync(path));
    }

    [Theory]
    [InlineData(StorageFileKind.Image, false)]
    [InlineData(StorageFileKind.Image, true)]
    [InlineData(StorageFileKind.Pdf, false)]
    [InlineData(StorageFileKind.Pdf, true)]
    public async Task Reconciliation_SkipsLongNamesAndLogsStableHashesWithoutLeakingNames(StorageFileKind kind, bool temporary)
    {
        await using var context = CreateContext();
        var extension = kind == StorageFileKind.Image ? ".png" : ".pdf";
        var suffix = temporary ? $"{extension}.{Guid.NewGuid():N}.tmp" : extension;
        var prefix = temporary ? "." : string.Empty;
        var validName = prefix + new string('a', StorageFileNames.MaxLength - prefix.Length - suffix.Length) + suffix;
        var firstLongName = prefix + "x" + validName[prefix.Length..];
        var secondLongName = prefix + "y" + validName[prefix.Length..];
        var root = Path.Combine(_root, kind == StorageFileKind.Image ? "images" : "pdfs");
        foreach (var name in new[] { firstLongName, validName, secondLongName })
        {
            await File.WriteAllTextAsync(Path.Combine(root, name), "orphan content");
        }

        var logger = new Mock<ILogger<StorageReconciliation>>();
        var reconciliation = new StorageReconciliation(context, Files(), _clock.Object, logger.Object);
        Assert.Equal(1, await reconciliation.ScanAsync(CancellationToken.None));
        Assert.Equal(1, await reconciliation.ScanAsync(CancellationToken.None));

        Assert.Equal(validName, (await context.StorageReconciliationFindings.SingleAsync()).FileName);
        var logs = logger.Invocations.Where(invocation => invocation.Method.Name == "Log").ToList();
        var entries = logs.Select(invocation => invocation.Arguments[2])
            .OfType<IEnumerable<KeyValuePair<string, object?>>>()
            .Where(state => state.Any(value => value.Key == "FileNameHash"))
            .Select(state => state.ToDictionary(value => value.Key, value => value.Value)).ToList();
        Assert.Equal(4, entries.Count);
        foreach (var name in new[] { firstLongName, secondLongName })
        {
            var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(name)));
            Assert.Equal(2, entries.Count(entry => Equals(entry["FileNameHash"], expectedHash)));
            Assert.Equal("orphan content", await File.ReadAllTextAsync(Path.Combine(root, name)));
        }

        Assert.All(entries, entry => Assert.Equal(kind, entry["FileKind"]));
        var text = string.Join('\n', logs.Select(invocation => invocation.Arguments[2].ToString()));
        Assert.DoesNotContain(firstLongName, text);
        Assert.DoesNotContain(secondLongName, text);
        Assert.DoesNotContain(root, text);
        Assert.DoesNotContain("orphan content", text);
        Assert.All(logs, invocation => Assert.Null(invocation.Arguments[3]));
    }
}
