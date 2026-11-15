using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageLifecycleTests
{
    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public async Task Reconciliation_SavesFindingsPerBatchRatherThanPerFile(StorageFileKind kind)
    {
        const int fileCount = 205;
        await using (var seed = CreateContext())
        {
            for (var index = 0; index < fileCount; index++)
            {
                if (kind == StorageFileKind.Image)
                {
                    seed.Images.Add(NewImage($"missing-{index}"));
                }
                else
                {
                    seed.PdfReports.Add(new PdfReport { Name = $"Missing {index}", BlobName = $"missing-{index}.pdf" });
                }

                var name = $"orphan-{index}.{(kind == StorageFileKind.Image ? "png" : "pdf")}";
                await File.WriteAllTextAsync(Files().GetPath(kind, name), "orphan content");
            }

            await seed.SaveChangesAsync();
        }

        List<int> batchSizes = [];
        var interceptor = new StorageSaveInterceptor(before: database =>
        {
            batchSizes.Add(database.ChangeTracker.Entries<StorageReconciliationFinding>()
                .Count(entry => entry.State is EntityState.Added or EntityState.Modified));
        });
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions).AddInterceptors(interceptor).Options;
        await using var context = new VictoryCenterDbContext(options, _clock.Object);

        Assert.Equal(fileCount * 2, await Reconciliation(context).ScanAsync(CancellationToken.None));

        Assert.Equal(4, batchSizes.Count);
        Assert.Equal(fileCount * 2, batchSizes.Sum());
        Assert.All(batchSizes, size => Assert.InRange(size, 1, 200));
        Assert.Empty(context.ChangeTracker.Entries<StorageReconciliationFinding>());
        batchSizes.Clear();
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddMinutes(1));
        Assert.Equal(fileCount * 2, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Equal(4, batchSizes.Count);
        Assert.Equal(fileCount * 2, batchSizes.Sum());
        Assert.Equal(fileCount * 2, await context.StorageReconciliationFindings.CountAsync());
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public async Task Reconciliation_ResolvesStaleFindingsInBatches(StorageFileKind kind)
    {
        const int findingCount = 205;
        await using (var seed = CreateContext())
        {
            seed.StorageReconciliationFindings.AddRange(Enumerable.Range(0, findingCount).Select(index => new StorageReconciliationFinding
            {
                FileKind = kind, FileName = $"gone-{index}.{(kind == StorageFileKind.Image ? "png" : "pdf")}",
                Kind = StorageFindingKind.Unreferenced, FirstSeenAt = _now.AddDays(-2), LastSeenAt = _now.AddDays(-1)
            }));
            await seed.SaveChangesAsync();
        }

        var saves = 0;
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions)
            .AddInterceptors(new StorageSaveInterceptor(before: _ => saves++)).Options;
        await using var context = new VictoryCenterDbContext(options, _clock.Object);

        Assert.Equal(0, await Reconciliation(context).ScanAsync(CancellationToken.None));

        Assert.Equal(2, saves);
        Assert.Equal(findingCount, await context.StorageReconciliationFindings.CountAsync(finding => finding.ResolvedAt == _now));
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public async Task BatchReferences_MatchIndividualLookups(StorageFileKind kind)
    {
        await using var context = CreateContext();
        context.Images.AddRange(
            NewImage("shared"),
            new Image { BlobName = "shared", MimeType = "IMAGE/JPG" },
            new Image { BlobName = "shared", MimeType = "image/webp" },
            new Image { BlobName = "legacy", MimeType = "image/gif" });
        context.PdfReports.Add(new PdfReport { Name = "Dotted", BlobName = "report.v2.pdf" });
        await context.SaveChangesAsync();
        string[] names = ["shared.jpg", "shared.png", "shared.webp", "legacy.jpg", "legacy.png", "absent.png", "report.v2.pdf"];

        var referenced = (await StorageReferences.ReferencedNames(context, kind, names).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            Assert.Equal(await StorageReferences.ExistsAsync(context, kind, name, CancellationToken.None), referenced.Contains(name));
        }
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public async Task BatchProtection_MatchesExactDottedAndLegacyTemporaryLookups(StorageFileKind kind)
    {
        await using var context = CreateContext();
        foreach (var name in new[] { "exact.png", "report.v2.pdf", "legacy.jpg" })
        {
            var operation = Operation(name, StorageOperationKind.UploadCleanup);
            operation.FileKind = kind;
            context.StorageOperations.Add(operation);
        }

        var completed = Operation("completed.png");
        completed.FileKind = kind;
        completed.Status = StorageOperationStatus.Completed;
        context.StorageOperations.Add(completed);
        await context.SaveChangesAsync();
        string[] names =
        [
            "exact.png", "completed.png", "absent.png",
            $".report.v2.pdf.{Guid.NewGuid():N}.tmp", $".legacy.{Guid.NewGuid():N}.tmp",
            $".absent.{Guid.NewGuid():N}.tmp"
        ];

        var protectedNames = await StorageReferences.ProtectedNamesAsync(context, kind, names, CancellationToken.None);

        foreach (var name in names)
        {
            Assert.Equal(await StorageReferences.IsProtectedAsync(context, kind, name, CancellationToken.None), protectedNames.Contains(name));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Reconciliation_BatchConcurrencyConflictDoesNotOverwriteCleanupLinkOrBlockHealthyFinding(bool keepConflicting)
    {
        Guid racedId;
        await using (var seed = CreateContext())
        {
            seed.Images.AddRange(NewImage("raced"), NewImage("healthy"));
            var raced = new StorageReconciliationFinding
            {
                FileName = "raced.png", Kind = StorageFindingKind.Missing, FirstSeenAt = _now.AddDays(-2), LastSeenAt = _now.AddDays(-1)
            };
            racedId = raced.Id;
            seed.StorageReconciliationFindings.AddRange(raced, new StorageReconciliationFinding
            {
                FileName = "healthy.png", Kind = StorageFindingKind.Missing, FirstSeenAt = _now.AddDays(-2), LastSeenAt = _now.AddDays(-1)
            });
            await seed.SaveChangesAsync();
        }

        Guid? latestLink = null;
        var conflicts = 0;
        var interceptor = new StorageSaveInterceptor(before: database =>
        {
            if ((!keepConflicting && conflicts > 0) || !database.ChangeTracker.Entries<StorageReconciliationFinding>()
                .Any(entry => entry.Entity.Id == racedId && entry.State == EntityState.Modified))
            {
                return;
            }

            using var winner = CreateContext();
            var finding = winner.StorageReconciliationFindings.Single(item => item.Id == racedId);
            latestLink = Guid.NewGuid();
            finding.CleanupOperationId = latestLink;
            winner.SaveChanges();
            conflicts++;
        });
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions).AddInterceptors(interceptor).Options;
        await using var context = new VictoryCenterDbContext(options, _clock.Object);

        Assert.Equal(2, await Reconciliation(context).ScanAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5)));

        await using var verification = CreateContext();
        var racedFinding = await verification.StorageReconciliationFindings.SingleAsync(finding => finding.Id == racedId);
        Assert.Equal(latestLink, racedFinding.CleanupOperationId);
        Assert.Equal(keepConflicting ? _now.AddDays(-1) : _now, racedFinding.LastSeenAt);
        Assert.Equal(_now, (await verification.StorageReconciliationFindings.SingleAsync(finding => finding.FileName == "healthy.png")).LastSeenAt);
        Assert.Equal(keepConflicting ? 2 : 1, conflicts);
        Assert.Empty(context.ChangeTracker.Entries<StorageReconciliationFinding>());
    }
}
