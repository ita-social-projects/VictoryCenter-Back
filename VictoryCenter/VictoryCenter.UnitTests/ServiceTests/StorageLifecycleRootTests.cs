using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageLifecycleTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EnumerationIterationFailure_PreservesRootKindAndCause(bool denied)
    {
        Exception cause = denied ? new UnauthorizedAccessException("private path") : new IOException("private path");
        var enumerator = new Mock<IEnumerator<string>>();
        enumerator.Setup(value => value.MoveNext()).Throws(cause);
        var method = typeof(LocalStorageFiles).GetMethod("MoveNext", BindingFlags.Static | BindingFlags.NonPublic)!;

        var invocation = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, [enumerator.Object, StorageFileKind.Pdf]));

        var exception = Assert.IsType<StorageRootUnavailableException>(invocation.InnerException);
        Assert.Equal(StorageFileKind.Pdf, exception.FileKind);
        Assert.Same(cause, exception.InnerException);
        Assert.DoesNotContain("private path", exception.Message);
    }

    [UnixFileSystemFact]
    public void RootPermissions_DistinguishRootOutageFromIndividualFileAccess()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var files = Files();
        var root = Path.Combine(_root, "images");
        var path = Path.Combine(root, "existing.png");
        File.WriteAllText(path, "image");
        var original = File.GetUnixFileMode(root);
        try
        {
            foreach (var mode in new[] { UnixFileMode.UserRead, UnixFileMode.UserExecute })
            {
                File.SetUnixFileMode(root, mode);
                Assert.Throws<StorageRootUnavailableException>(() => files.EnsureAvailable(StorageFileKind.Image));
                if (mode == UnixFileMode.UserRead)
                {
                    Assert.Throws<StorageRootUnavailableException>(() => files.Exists(StorageFileKind.Image, "existing.png"));
                }
                else
                {
                    Assert.Throws<StorageRootUnavailableException>(() => files.Enumerate(StorageFileKind.Image).ToList());
                }
            }
        }
        finally
        {
            File.SetUnixFileMode(root, original);
        }

        File.SetUnixFileMode(path, UnixFileMode.None);
        Assert.True(files.Exists(StorageFileKind.Image, "existing.png"));
        files.EnsureAvailable(StorageFileKind.Image);
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public void ScopedServices_DoNotRecreateAnUnavailableRoot(StorageFileKind kind)
    {
        var root = Path.Combine(_root, kind == StorageFileKind.Image ? "images" : "pdfs");
        Directory.Delete(root);

        _ = new BlobService(
            Options.Create(new BlobEnvironmentVariables { RootPath = _root, ImagesSubPath = "images" }),
            new HttpContextAccessor(),
            NullLogger<BlobService>.Instance);
        _ = new PdfService(
            Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }), new HttpContextAccessor());

        var exception = Assert.Throws<StorageRootUnavailableException>(() => Files());
        Assert.Equal(kind, exception.FileKind);
        Assert.NotNull(exception.InnerException);
        Assert.DoesNotContain(root, exception.Message);
        Assert.False(Directory.Exists(root));
    }

    [Theory]
    [InlineData(StorageFileKind.Image, "missing.png")]
    [InlineData(StorageFileKind.Pdf, "missing.pdf")]
    public void CachedStorageFiles_DistinguishMissingFileFromMissingRoot(StorageFileKind kind, string name)
    {
        var files = Files();
        Assert.False(files.Exists(kind, name));
        var root = Path.Combine(_root, kind == StorageFileKind.Image ? "images" : "pdfs");
        Directory.Delete(root);

        Assert.Throws<StorageRootUnavailableException>(() => files.Exists(kind, name));
        Assert.Throws<StorageRootUnavailableException>(() => files.Enumerate(kind).ToList());
        Assert.False(Directory.Exists(root));
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public void CachedStorageFiles_RootReplacedByFileIsClassifiedAndRecovers(StorageFileKind kind)
    {
        var files = Files();
        var root = Path.Combine(_root, kind == StorageFileKind.Image ? "images" : "pdfs");
        Directory.Delete(root);
        File.WriteAllText(root, "not a directory");

        var exception = Assert.Throws<StorageRootUnavailableException>(() => files.EnsureAvailable(kind));

        Assert.Equal(kind, exception.FileKind);
        Assert.IsType<IOException>(exception.InnerException);
        Assert.DoesNotContain(root, exception.Message);
        File.Delete(root);
        Directory.CreateDirectory(root);
        files.EnsureAvailable(kind);
        Assert.False(files.Exists(kind, kind == StorageFileKind.Image ? "absent.png" : "absent.pdf"));
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public async Task Reconciliation_UnavailableRootDoesNotCreateFalseMissingFindings(StorageFileKind kind)
    {
        await using var context = CreateContext();
        context.Images.AddRange(NewImage("first"), NewImage("second"));
        context.PdfReports.Add(new PdfReport { Name = "Missing report", BlobName = "missing.pdf" });
        await context.SaveChangesAsync();
        var reconciliation = Reconciliation(context);
        Directory.Delete(Path.Combine(_root, kind == StorageFileKind.Image ? "images" : "pdfs"));

        await Assert.ThrowsAsync<StorageRootUnavailableException>(() => reconciliation.ScanAsync(CancellationToken.None));

        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Theory]
    [InlineData(StorageFileKind.Image, false)]
    [InlineData(StorageFileKind.Pdf, false)]
    [InlineData(StorageFileKind.Image, true)]
    [InlineData(StorageFileKind.Pdf, true)]
    public async Task Reconciliation_RootDisappearingDuringScanStopsWithoutFloodingFindings(StorageFileKind kind, bool replaceWithFile)
    {
        await using (var seed = CreateContext())
        {
            const int referenceCount = 201;
            if (kind == StorageFileKind.Image)
            {
                seed.Images.AddRange(Enumerable.Range(0, referenceCount).Select(index => NewImage($"missing-{index}")));
            }
            else
            {
                seed.PdfReports.AddRange(Enumerable.Range(0, referenceCount)
                    .Select(index => new PdfReport { Name = $"Missing {index}", BlobName = $"missing-{index}.pdf" }));
            }

            await seed.SaveChangesAsync();
        }

        var removed = false;
        var root = Path.Combine(_root, kind == StorageFileKind.Image ? "images" : "pdfs");
        var interceptor = new StorageSaveInterceptor(after: database =>
        {
            if (!removed && database.ChangeTracker.Entries<StorageReconciliationFinding>().Any())
            {
                Directory.Delete(root);
                if (replaceWithFile)
                {
                    File.WriteAllText(root, "not a directory");
                }

                removed = true;
            }
        });
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions).AddInterceptors(interceptor).Options;
        await using var context = new VictoryCenterDbContext(options);

        var logger = new Mock<ILogger<StorageReconciliation>>();
        var reconciliation = new StorageReconciliation(context, Files(), _clock.Object, logger.Object);
        await Assert.ThrowsAsync<StorageRootUnavailableException>(() => reconciliation.ScanAsync(CancellationToken.None));
        logger.Verify(
            value => value.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);

        Assert.True(removed);
        Assert.Equal(200, await context.StorageReconciliationFindings.CountAsync());
        Assert.False(Directory.Exists(root));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public async Task PdfUpload_BlankNamesGenerateIndependentImmutableIdentifiers(string? name)
    {
        using var provider = new ServiceCollection().AddScoped(_ => CreateContext()).BuildServiceProvider();
        var storage = new PdfStorageService(
            new PdfService(Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }), new HttpContextAccessor()),
            Reservations(provider));
        async Task<string> Upload()
        {
            await using var bytes = new MemoryStream("%PDF-1.4"u8.ToArray());
            var file = new FormFile(bytes, 0, bytes.Length, "file", "report.pdf")
            {
                Headers = new HeaderDictionary(), ContentType = "application/pdf"
            };
            return await storage.UploadPdfAsync(file, name);
        }

        var first = await Upload();
        var second = await Upload();

        Assert.NotEqual(first, second);
        Assert.True(Guid.TryParseExact(Path.GetFileNameWithoutExtension(first), "N", out _));
        Assert.True(Guid.TryParseExact(Path.GetFileNameWithoutExtension(second), "N", out _));
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Pdf, first)));
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Pdf, second)));
        await using var verification = CreateContext();
        Assert.Equal(2, await verification.StorageOperations.CountAsync());
        Assert.DoesNotContain(await verification.StorageOperations.ToListAsync(), operation => operation.FileName == ".pdf");
    }
}
