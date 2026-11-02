using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageLifecycleTests
{
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

        Assert.Throws<DirectoryNotFoundException>(() => Files());
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

        Assert.Throws<DirectoryNotFoundException>(() => files.Exists(kind, name));
        Assert.Throws<DirectoryNotFoundException>(() => files.Enumerate(kind).ToList());
        Assert.False(Directory.Exists(root));
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

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => reconciliation.ScanAsync(CancellationToken.None));

        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    [Theory]
    [InlineData(StorageFileKind.Image)]
    [InlineData(StorageFileKind.Pdf)]
    public async Task Reconciliation_RootDisappearingDuringScanStopsWithoutFloodingFindings(StorageFileKind kind)
    {
        await using (var seed = CreateContext())
        {
            if (kind == StorageFileKind.Image)
            {
                seed.Images.AddRange(NewImage("first"), NewImage("second"), NewImage("third"));
            }
            else
            {
                seed.PdfReports.AddRange(
                    new PdfReport { Name = "First", BlobName = "first.pdf" },
                    new PdfReport { Name = "Second", BlobName = "second.pdf" },
                    new PdfReport { Name = "Third", BlobName = "third.pdf" });
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
                removed = true;
            }
        });
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions).AddInterceptors(interceptor).Options;
        await using var context = new VictoryCenterDbContext(options);

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => Reconciliation(context).ScanAsync(CancellationToken.None));

        Assert.True(removed);
        Assert.Single(await context.StorageReconciliationFindings.ToListAsync());
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
