using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageLifecycleTests : IDisposable
{
    private static readonly string[] SharedInventoryNames = ["shared", "other"];
    private static readonly string[] DeletedImageNames = ["first.png", "second.png"];
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"vc-storage-{Guid.NewGuid():N}");
    private readonly DbContextOptions<VictoryCenterDbContext> _databaseOptions =
        new DbContextOptionsBuilder<VictoryCenterDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
    private readonly Mock<TimeProvider> _clock = new();
    private readonly StorageLifecycleOptions _options = new();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    public StorageLifecycleTests()
    {
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now);
        Directory.CreateDirectory(Path.Combine(_root, "images"));
        Directory.CreateDirectory(Path.Combine(_root, "pdfs"));
    }

    [Fact]
    public async Task MetadataDelete_QueuesCleanupWithoutDeletingFile()
    {
        await using var context = CreateContext();
        var image = NewImage("existing");
        context.Images.Add(image);
        await context.SaveChangesAsync();
        var path = Files().GetPath(StorageFileKind.Image, "existing.png");
        await File.WriteAllTextAsync(path, "original");

        context.Images.Remove(image);
        await context.SaveChangesAsync();

        var operation = Assert.Single(await context.StorageOperations.ToListAsync());
        Assert.Equal(StorageOperationKind.Delete, operation.Kind);
        Assert.Equal("existing.png", operation.FileName);
        Assert.Equal("original", await File.ReadAllTextAsync(path));
        Assert.Empty(await context.Images.ToListAsync());
    }

    [Fact]
    public async Task Replacement_QueuesPreviousExtensionAndChangesConcurrencyVersion()
    {
        await using var context = CreateContext();
        var image = NewImage("old");
        context.Images.Add(image);
        await context.SaveChangesAsync();
        var version = image.StorageVersion;

        image.BlobName = "new";
        image.MimeType = "image/jpeg";
        await context.SaveChangesAsync();

        Assert.Equal("old.png", Assert.Single(await context.StorageOperations.ToListAsync()).FileName);
        Assert.NotEqual(version, image.StorageVersion);
    }

    [Fact]
    public async Task UploadIntent_IsCompletedWithMetadata()
    {
        await using var context = CreateContext();
        var operation = Operation("new.png", StorageOperationKind.UploadCleanup);
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        context.Images.Add(NewImage("new"));

        await context.SaveChangesAsync();

        Assert.Equal(StorageOperationStatus.Completed, operation.Status);
        Assert.NotNull(operation.CompletedAt);
    }

    [Fact]
    public void SynchronousSave_CompletesSharedUploadReservationOnce()
    {
        using var context = CreateContext();
        var operation = Operation("shared.png", StorageOperationKind.UploadCleanup);
        context.StorageOperations.Add(operation);
        context.SaveChanges();
        context.Images.AddRange(NewImage("shared"), NewImage("shared"));

        context.SaveChanges();

        Assert.Equal(2, context.Images.Count());
        Assert.Equal(StorageOperationStatus.Completed, operation.Status);
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public void Inventory_TranslatesGroupingAndOrderedPagingToSql(StorageFileKind kind)
    {
        using var context = new VictoryCenterDbContext(new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=unused;Database=QueryTranslationOnly;Integrated Security=True").Options);

        var sql = StorageReferences.Inventory(context, kind).Where(reference => reference.Id > 10)
            .OrderBy(reference => reference.Id).Take(25).ToQueryString();

        Assert.Contains("GROUP BY", sql);
        Assert.Contains("MIN(", sql);
        Assert.Contains("ORDER BY", sql);
        Assert.Contains("HAVING", sql);
    }

    [Fact]
    public async Task Inventory_GroupsSharedFilesAndOrdersByFirstMetadataId()
    {
        await using var context = CreateContext();
        context.Images.AddRange(NewImage("shared"), NewImage("other"), NewImage("shared"));
        await context.SaveChangesAsync();

        var inventory = await StorageReferences.Inventory(context, StorageFileKind.Image)
            .OrderBy(reference => reference.Id).ToListAsync();

        Assert.Equal(SharedInventoryNames, inventory.Select(reference => reference.Name));
        var nextPage = await StorageReferences.Inventory(context, StorageFileKind.Image)
            .Where(reference => reference.Id > inventory[0].Id).OrderBy(reference => reference.Id).Take(1).ToListAsync();
        Assert.Equal("other", Assert.Single(nextPage).Name);
        Assert.Equal(2, await Reconciliation(context).ScanAsync(CancellationToken.None));
    }

    [Fact]
    public async Task Inventory_GroupsEquivalentJpegMimeTypesAsOneFile()
    {
        await using var context = CreateContext();
        context.Images.AddRange(
            new Image { BlobName = "shared", MimeType = "image/jpeg" },
            new Image { BlobName = "shared", MimeType = "image/JPG" },
            new Image { BlobName = "shared", MimeType = "image/gif" });
        await context.SaveChangesAsync();

        Assert.Equal(1, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Equal("shared.jpg", (await context.StorageReconciliationFindings.SingleAsync()).FileName);
    }

    [Theory]
    [InlineData(StorageOperationStatus.Processing)]
    [InlineData(StorageOperationStatus.Completed)]
    [InlineData(StorageOperationStatus.Failed)]
    public async Task ReclaimedUpload_CannotBeAttachedToNewMetadata(StorageOperationStatus status)
    {
        await using var context = CreateContext();
        var operation = Operation("expired.png", StorageOperationKind.UploadCleanup);
        operation.Status = status;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        context.Images.Add(NewImage("expired"));

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => context.SaveChangesAsync());
        await using var verification = CreateContext();
        Assert.Empty(await verification.Images.ToListAsync());
    }

    [Theory]
    [InlineData(StorageFileKind.Image, "orphan.png")]
    [InlineData(StorageFileKind.Pdf, "orphan.pdf")]
    public async Task Processor_DeletesUnreferencedFileAndAcknowledges(StorageFileKind kind, string name)
    {
        await using var context = CreateContext();
        var operation = Operation(name);
        operation.FileKind = kind;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        var path = Files().GetPath(kind, name);
        await File.WriteAllTextAsync(path, "test bytes");

        Assert.Equal(1, await Processor(context).ProcessBatchAsync(CancellationToken.None));

        Assert.False(File.Exists(path));
        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Completed, saved.Status);
        Assert.Equal(1, saved.Attempts);
        Assert.NotNull(saved.CompletedAt);
    }

    [Fact]
    public async Task Processor_MissingFileIsIdempotent()
    {
        await using var context = CreateContext();
        context.StorageOperations.Add(Operation("absent.png"));
        await context.SaveChangesAsync();

        Assert.Equal(1, await Processor(context).ProcessBatchAsync(CancellationToken.None));
        Assert.Equal(0, await Processor(context).ProcessBatchAsync(CancellationToken.None));
        Assert.Equal(StorageOperationStatus.Completed, (await context.StorageOperations.SingleAsync()).Status);
    }

    [Fact]
    public async Task Processor_RecoversWhenFileDeletedButAcknowledgementFails()
    {
        await using (var seed = CreateContext())
        {
            seed.StorageOperations.Add(Operation("deleted.png"));
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, "deleted.png");
        await File.WriteAllTextAsync(path, "test bytes");
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions)
            .AddInterceptors(new RejectAcknowledgement()).Options;
        await using (var failed = new VictoryCenterDbContext(options))
        {
            Assert.Equal(0, await Processor(failed).ProcessBatchAsync(CancellationToken.None));
            Assert.Empty(failed.ChangeTracker.Entries());
        }

        Assert.False(File.Exists(path));
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddSeconds(_options.LeaseSeconds + 1));
        await using var recovery = CreateContext();
        Assert.Equal(1, await Processor(recovery).ProcessBatchAsync(CancellationToken.None));
        var operation = await recovery.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Completed, operation.Status);
        Assert.Equal(2, operation.Attempts);
    }

    [Theory]
    [InlineData("image/png", "live.png")]
    [InlineData("image/PNG", "live.png")]
    [InlineData("image/jpeg", "live.jpg")]
    [InlineData("image/JPG", "live.jpg")]
    [InlineData("image/WEBP", "live.webp")]
    [InlineData("image/gif", "live.jpg")]
    [InlineData("application/octet-stream", "live.jpg")]
    [InlineData("", "live.jpg")]
    public async Task Processor_DoesNotDeleteReferencedImage(string mimeType, string fileName)
    {
        await using var context = CreateContext();
        context.Images.Add(new Image { BlobName = "live", MimeType = mimeType });
        context.StorageOperations.Add(Operation(fileName));
        await context.SaveChangesAsync();
        var path = Files().GetPath(StorageFileKind.Image, fileName);
        await File.WriteAllTextAsync(path, "keep");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal("keep", await File.ReadAllTextAsync(path));
        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Failed, saved.Status);
        Assert.Equal("FileStillReferenced", saved.LastErrorCode);
    }

    [Fact]
    public async Task Processor_DoesNotDeleteReferencedPdf()
    {
        await using var context = CreateContext();
        context.PdfReports.Add(new PdfReport { Name = "Live report", BlobName = "live.pdf" });
        var operation = Operation("live.pdf");
        operation.FileKind = StorageFileKind.Pdf;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        var path = Files().GetPath(StorageFileKind.Pdf, "live.pdf");
        await File.WriteAllTextAsync(path, "keep pdf");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal("keep pdf", await File.ReadAllTextAsync(path));
        Assert.Equal("FileStillReferenced", (await context.StorageOperations.SingleAsync()).LastErrorCode);
    }

    [Fact]
    public async Task ExpiredUpload_RemovesPartialTemporaryFile()
    {
        await using var context = CreateContext();
        context.StorageOperations.Add(Operation("abandoned.png", StorageOperationKind.UploadCleanup));
        await context.SaveChangesAsync();
        var temporaryPath = Files().GetPath(StorageFileKind.Image, $".abandoned.png.{Guid.NewGuid():N}.tmp");
        await File.WriteAllTextAsync(temporaryPath, "partial bytes");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.False(File.Exists(temporaryPath));
        Assert.Equal(StorageOperationStatus.Completed, (await context.StorageOperations.SingleAsync()).Status);
    }

    [Fact]
    public async Task ExpiredUpload_DoesNotDeleteAnotherExtensionsTemporaryFile()
    {
        await using var context = CreateContext();
        var active = Operation("shared.png", StorageOperationKind.UploadCleanup);
        active.NextAttemptAt = _now.AddHours(24);
        context.StorageOperations.AddRange(Operation("shared.jpg", StorageOperationKind.UploadCleanup), active);
        await context.SaveChangesAsync();
        var expiredPath = Files().GetPath(StorageFileKind.Image, $".shared.jpg.{Guid.NewGuid():N}.tmp");
        var activePath = Files().GetPath(StorageFileKind.Image, $".shared.png.{Guid.NewGuid():N}.tmp");
        await File.WriteAllTextAsync(expiredPath, "expired bytes");
        await File.WriteAllTextAsync(activePath, "active bytes");

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.False(File.Exists(expiredPath));
        Assert.Equal("active bytes", await File.ReadAllTextAsync(activePath));
    }

    [Fact]
    public async Task ImageVersion_RejectsCompetingReplacement()
    {
        await using var first = CreateContext();
        first.Images.Add(NewImage("original"));
        await first.SaveChangesAsync();
        await using var second = CreateContext();
        var stale = await second.Images.SingleAsync();
        var current = await first.Images.SingleAsync();
        current.BlobName = "winner";
        await first.SaveChangesAsync();
        stale.BlobName = "loser";

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
        await using var verification = CreateContext();
        Assert.Equal("winner", (await verification.Images.SingleAsync()).BlobName);
    }

    [Fact]
    public async Task FailedPdfWrite_PreservesExistingFileAndRemovesTemporaryFile()
    {
        var path = Files().GetPath(StorageFileKind.Pdf, "preserved.pdf");
        await File.WriteAllTextAsync(path, "original pdf");
        var file = new Mock<IFormFile>();
        file.SetupGet(value => value.Length).Returns(8);
        file.SetupGet(value => value.ContentType).Returns("application/pdf");
        file.SetupGet(value => value.FileName).Returns("report.pdf");
        file.Setup(value => value.OpenReadStream()).Returns(() => new MemoryStream("%PDF-1.4"u8.ToArray()));
        file.Setup(value => value.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>(async (stream, token) =>
            {
                await stream.WriteAsync("partial"u8.ToArray(), token);
                throw new IOException("Simulated interrupted upload");
            });
        var service = new PdfService(
            Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }), new HttpContextAccessor());

        await Assert.ThrowsAsync<BlobFileSystemException>(() => service.UploadPdfAsync(file.Object, "preserved"));

        Assert.Equal("original pdf", await File.ReadAllTextAsync(path));
        Assert.Single(Directory.EnumerateFiles(Path.Combine(_root, "pdfs")));
    }

    [Fact]
    public async Task Processor_RetriesFailureWithBackoffThenStops()
    {
        await using var context = CreateContext();
        _options.MaxAttempts = 2;
        context.StorageOperations.Add(Operation("directory.png"));
        await context.SaveChangesAsync();
        Directory.CreateDirectory(Files().GetPath(StorageFileKind.Image, "directory.png"));

        await Processor(context).ProcessBatchAsync(CancellationToken.None);
        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Pending, saved.Status);
        Assert.True(saved.NextAttemptAt > _now);
        Assert.NotNull(saved.LastErrorCode);
        Assert.Equal(0, await Processor(context).ProcessBatchAsync(CancellationToken.None));

        saved = await context.StorageOperations.SingleAsync();
        saved.NextAttemptAt = _now;
        await context.SaveChangesAsync();
        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal(StorageOperationStatus.Failed, (await context.StorageOperations.SingleAsync()).Status);
        Assert.Equal(0, await Processor(context).ProcessBatchAsync(CancellationToken.None));
    }

    [Fact]
    public async Task Processor_UsesConfiguredRetryDelayAndCapsBackoff()
    {
        await using var context = CreateContext();
        _options.RetryDelaySeconds = 7;
        _options.MaxRetryDelaySeconds = 9;
        context.StorageOperations.Add(Operation("blocked.png"));
        await context.SaveChangesAsync();
        Directory.CreateDirectory(Files().GetPath(StorageFileKind.Image, "blocked.png"));

        await Processor(context).ProcessBatchAsync(CancellationToken.None);
        var operation = await context.StorageOperations.SingleAsync();
        Assert.Equal(_now.AddSeconds(7), operation.NextAttemptAt);
        operation.NextAttemptAt = _now;
        await context.SaveChangesAsync();

        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.Equal(_now.AddSeconds(9), (await context.StorageOperations.SingleAsync()).NextAttemptAt);
    }

    [Fact]
    public async Task Processor_RecoversExpiredLeaseButDoesNotStealActiveLease()
    {
        await using var context = CreateContext();
        var expired = Operation("expired.png");
        expired.Status = StorageOperationStatus.Processing;
        var active = Operation("active.png");
        active.Status = StorageOperationStatus.Processing;
        active.NextAttemptAt = _now.AddMinutes(5);
        context.StorageOperations.AddRange(expired, active);
        await context.SaveChangesAsync();

        Assert.Equal(1, await Processor(context).ProcessBatchAsync(CancellationToken.None));
        Assert.Equal(StorageOperationStatus.Processing, (await context.StorageOperations.SingleAsync(item => item.Id == active.Id)).Status);
    }

    [Fact]
    public async Task Processor_DoesNotTouchUnexpiredUploadIntent()
    {
        await using var context = CreateContext();
        var operation = Operation("upload.png", StorageOperationKind.UploadCleanup);
        operation.NextAttemptAt = _now.AddHours(24);
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();

        Assert.Equal(0, await Processor(context).ProcessBatchAsync(CancellationToken.None));
    }

    [Fact]
    public async Task OperationVersion_RejectsStaleWorkerAcknowledgement()
    {
        await using var first = CreateContext();
        first.StorageOperations.Add(Operation("race.png"));
        await first.SaveChangesAsync();
        await using var second = CreateContext();
        var stale = await second.StorageOperations.SingleAsync();
        var current = await first.StorageOperations.SingleAsync();
        current.Version = Guid.NewGuid();
        await first.SaveChangesAsync();
        stale.Status = StorageOperationStatus.Completed;
        stale.Version = Guid.NewGuid();

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
    }

    [Fact]
    public async Task Reconciliation_FindsMissingAndOrphanButDoesNotDeleteAnything()
    {
        await using var context = CreateContext();
        context.Images.Add(NewImage("missing"));
        await context.SaveChangesAsync();
        var orphan = Files().GetPath(StorageFileKind.Pdf, "unreferenced.pdf");
        await File.WriteAllTextAsync(orphan, "test pdf bytes");

        Assert.Equal(2, await Reconciliation(context).ScanAsync(CancellationToken.None));

        var findings = await context.StorageReconciliationFindings.ToListAsync();
        Assert.Contains(findings, finding => finding.Kind == StorageFindingKind.Missing && finding.FileName == "missing.png");
        Assert.Contains(findings, finding => finding.Kind == StorageFindingKind.Unreferenced && finding.FileName == "unreferenced.pdf");
        Assert.True(File.Exists(orphan));
        Assert.Empty(await context.StorageOperations.ToListAsync());
    }

    [Fact]
    public async Task Startup_InitializesEmptyStorageOnceBeforeTheFirstScan()
    {
        var root = Path.Combine(_root, "empty");
        var images = Options.Create(new BlobEnvironmentVariables { RootPath = root, ImagesSubPath = "images" });
        var pdfs = Options.Create(new PdfEnvironmentVariables { RootPath = root, PdfSubPath = "pdfs" });
        using var services = new ServiceCollection().AddSingleton(images).AddSingleton(pdfs).BuildServiceProvider();
        var app = new ApplicationBuilder(services);
        app.InitializeStorage();
        var sentinel = Path.Combine(images.Value.FullPath, "existing.png");
        await File.WriteAllTextAsync(sentinel, "existing bytes");
        app.InitializeStorage();
        Assert.Equal("existing bytes", await File.ReadAllTextAsync(sentinel));
        File.Delete(sentinel);
        var files = new LocalStorageFiles(
            images,
            pdfs);
        await using var context = CreateContext();
        var reconciliation = new StorageReconciliation(context, files, _clock.Object, NullLogger<StorageReconciliation>.Instance);

        Assert.Equal(0, await reconciliation.ScanAsync(CancellationToken.None));
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
        Assert.True(Directory.Exists(Path.Combine(root, "images")));
        Assert.True(Directory.Exists(Path.Combine(root, "pdfs")));
    }

    [Fact]
    public async Task Reconciliation_DoesNotReportActiveUploadsAsOrphans()
    {
        await using var context = CreateContext();
        var operation = Operation("pending.pdf", StorageOperationKind.UploadCleanup);
        operation.FileKind = StorageFileKind.Pdf;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Pdf, "pending.pdf"), "upload");

        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Pdf, $".pending.pdf.{Guid.NewGuid():N}.tmp"), "partial upload");
        Assert.Equal(0, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Fact]
    public async Task Reconciliation_ProtectsTemporaryFilesForDottedIdentifiers()
    {
        await using var context = CreateContext();
        var operation = Operation("report.v2.pdf", StorageOperationKind.UploadCleanup);
        operation.FileKind = StorageFileKind.Pdf;
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Pdf, $".report.v2.pdf.{Guid.NewGuid():N}.tmp"), "partial");

        Assert.Equal(0, await Reconciliation(context).ScanAsync(CancellationToken.None));
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Fact]
    public async Task Reconciliation_ResolvesFindingAfterFileRestored()
    {
        await using var context = CreateContext();
        context.Images.Add(NewImage("restored"));
        await context.SaveChangesAsync();
        await Reconciliation(context).ScanAsync(CancellationToken.None);
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, "restored.png"), "restored");
        await Reconciliation(context).ScanAsync(CancellationToken.None);

        Assert.NotNull((await context.StorageReconciliationFindings.SingleAsync()).ResolvedAt);
    }

    [Fact]
    public async Task Cleanup_EligibleAuditedRequestIsDeletedOnlyByProcessor()
    {
        await using var context = CreateContext();
        var path = Files().GetPath(StorageFileKind.Image, "old.png");
        await File.WriteAllTextAsync(path, "old orphan");
        File.SetLastWriteTimeUtc(path, _now.AddDays(-3).UtcDateTime);
        var operation = Operation("old.png", StorageOperationKind.OrphanCleanup);
        operation.RequestedBy = "admin-1";
        context.StorageOperations.Add(operation);
        context.StorageReconciliationFindings.Add(new StorageReconciliationFinding
        {
            FileKind = StorageFileKind.Image, FileName = "old.png", Kind = StorageFindingKind.Unreferenced,
            FirstSeenAt = _now.AddDays(-2), LastSeenAt = _now, CleanupOperationId = operation.Id
        });
        await context.SaveChangesAsync();

        Assert.True(File.Exists(path));
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddMinutes(1));
        await Reconciliation(context).ScanAsync(CancellationToken.None);
        Assert.Null((await context.StorageReconciliationFindings.SingleAsync()).ResolvedAt);
        await Processor(context).ProcessBatchAsync(CancellationToken.None);

        Assert.False(File.Exists(path));
        var saved = await context.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Completed, saved.Status);
        Assert.Equal("admin-1", saved.RequestedBy);
    }

    [Theory]
    [InlineData(".tmp")]
    [InlineData("..tmp")]
    [InlineData(".report.not-a-guid.tmp")]
    [InlineData("report.pdf")]
    public void TemporaryFile_RejectsUnmanagedNames(string name)
    {
        Assert.Null(StorageFileNames.TemporaryOwner(name));
    }

    [Theory]
    [InlineData("../outside.png")]
    [InlineData("..\\outside.png")]
    [InlineData("/outside.png")]
    [InlineData("..")]
    public void Paths_RejectTraversal(string name)
    {
        Assert.Throws<ArgumentException>(() => Files().GetPath(StorageFileKind.Image, name));
    }

    [Fact]
    public async Task Reservation_IsPersistedBeforeUploadAndRejectsIdentifierReuse()
    {
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();
        var reservations = Reservations(provider);
        await reservations.ReserveAsync(StorageFileKind.Image, "reserved.png");
        await using var verification = CreateContext();
        Assert.Equal(StorageOperationStatus.Pending, (await verification.StorageOperations.SingleAsync()).Status);
        await Assert.ThrowsAsync<IOException>(() => reservations.ReserveAsync(StorageFileKind.Image, "reserved.png"));
    }

    [Fact]
    public async Task FailedUpload_LeavesPersistedCleanupIntent()
    {
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();
        var reservations = Reservations(provider);
        var rawStorage = new BlobService(
            Options.Create(new BlobEnvironmentVariables { RootPath = _root, ImagesSubPath = "images" }),
            new HttpContextAccessor(),
            NullLogger<BlobService>.Instance);
        var storage = new ImageStorageService(rawStorage, reservations);

        await Assert.ThrowsAnyAsync<BlobStorageException>(() => storage.SaveFileInStorageAsync("invalid!", "failed", "image/png"));

        await using var verification = CreateContext();
        var intent = await verification.StorageOperations.SingleAsync();
        Assert.Equal("failed.png", intent.FileName);
        Assert.Equal(StorageOperationStatus.Pending, intent.Status);
        Assert.Empty(Directory.EnumerateFiles(Path.Combine(_root, "images")));
    }

    [Fact]
    public async Task PdfUpload_ReservesTheActualFilenameIncludingDottedIdentifiers()
    {
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();
        var rawStorage = new PdfService(
            Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }), new HttpContextAccessor());
        var storage = new PdfStorageService(rawStorage, Reservations(provider));
        await using var bytes = new MemoryStream("%PDF-1.4"u8.ToArray());
        var file = new FormFile(bytes, 0, bytes.Length, "file", "report.pdf") { Headers = new HeaderDictionary(), ContentType = "application/pdf" };

        var name = await storage.UploadPdfAsync(file, "report.v2.pdf");

        Assert.Equal("report.v2.pdf", name);
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Pdf, name)));
        await using var verification = CreateContext();
        Assert.Equal(name, (await verification.StorageOperations.SingleAsync()).FileName);
    }

    public void Dispose()
    {
        Directory.Delete(_root, recursive: true);
        GC.SuppressFinalize(this);
    }

    private static Image NewImage(string name) => new() { BlobName = name, MimeType = "image/png" };

    private StorageOperation Operation(string name, StorageOperationKind kind = StorageOperationKind.Delete) => new()
    {
        FileName = name,
        FileKind = StorageFileKind.Image,
        Kind = kind,
        NextAttemptAt = _now.AddMinutes(-1)
    };

    private VictoryCenterDbContext CreateContext() => new(_databaseOptions, _clock.Object);

    private StorageUploadReservations Reservations(ServiceProvider provider) => new(
        provider.GetRequiredService<IServiceScopeFactory>(), Files(), Options.Create(_options), _clock.Object);

    private LocalStorageFiles Files() => new(
        Options.Create(new BlobEnvironmentVariables { RootPath = _root, ImagesSubPath = "images" }),
        Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }));

    private StorageOperationProcessor Processor(VictoryCenterDbContext context) => new(
        context, Files(), Options.Create(_options), _clock.Object, NullLogger<StorageOperationProcessor>.Instance);

    private StorageReconciliation Reconciliation(VictoryCenterDbContext context) => new(
        context, Files(), _clock.Object, NullLogger<StorageReconciliation>.Instance);

    private sealed class RejectAcknowledgement : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<StorageOperation>().Any(entry => entry.Entity.Status == StorageOperationStatus.Completed))
            {
                throw new DbUpdateException("Simulated acknowledgement failure.");
            }

            return ValueTask.FromResult(result);
        }
    }
}
