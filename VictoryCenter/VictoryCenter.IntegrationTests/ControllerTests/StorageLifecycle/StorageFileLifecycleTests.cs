using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils.Images;

namespace VictoryCenter.IntegrationTests.ControllerTests.StorageLifecycle;

public class StorageFileLifecycleTests(IntegrationTestDbFixture fixture) : BaseTestClass(fixture)
{
    [Fact]
    public async Task Image_CreateReplaceAndDelete_ShouldKeepCommittedFilesUntilCleanup()
    {
        using var create = await Fixture.HttpClient.PostAsJsonAsync("/api/Image", new CreateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/jpeg"), MimeType = "image/jpeg"
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var original = (await create.Content.ReadFromJsonAsync<ImageDto>())!;
        var oldName = StorageFileNames.Image(original.BlobName, original.MimeType);
        var oldPath = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, oldName);
        var bytes = await File.ReadAllBytesAsync(oldPath);
        var reservation = await Fixture.DbContext.StorageOperations.AsNoTracking().SingleAsync(item => item.FileName == oldName);
        Assert.Equal(StorageOperationStatus.Completed, reservation.Status);

        using var update = await Fixture.HttpClient.PutAsJsonAsync($"/api/Image/{original.Id}", new UpdateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png"), MimeType = "image/png"
        });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var replacement = (await update.Content.ReadFromJsonAsync<ImageDto>())!;
        var newName = StorageFileNames.Image(replacement.BlobName, replacement.MimeType);
        var newPath = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, newName);
        Assert.NotEqual(original.BlobName, replacement.BlobName);
        Assert.NotEqual(original.Url, replacement.Url);
        Assert.Equal(bytes, await File.ReadAllBytesAsync(oldPath));
        Assert.True(File.Exists(newPath));
        var deletion = await Fixture.DbContext.StorageOperations.AsNoTracking()
            .SingleAsync(item => item.FileName == oldName && item.Kind == StorageOperationKind.Delete);
        Assert.Equal(StorageOperationStatus.Pending, deletion.Status);
        using var read = await Fixture.HttpClient.GetAsync($"/api/Image/{original.Id}");
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal(replacement.BlobName, (await read.Content.ReadFromJsonAsync<ImageDto>())!.BlobName);

        await Fixture.ProcessPendingStorageOperationsAsync();
        Assert.False(File.Exists(oldPath));
        Assert.True(File.Exists(newPath));
        using var delete = await Fixture.HttpClient.DeleteAsync($"/api/Image/{original.Id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);
        Assert.True(File.Exists(newPath));
        Assert.False(await Fixture.DbContext.Images.AsNoTracking().AnyAsync(item => item.Id == original.Id));
        await Fixture.ProcessPendingStorageOperationsAsync();
        Assert.False(File.Exists(newPath));
    }

    [Fact]
    public async Task Image_InvalidReplacement_ShouldPreserveFileMetadataAndQueue()
    {
        var image = await Fixture.DbContext.Images.AsNoTracking().FirstAsync();
        var path = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, StorageFileNames.Image(image.BlobName, image.MimeType));
        var bytes = await File.ReadAllBytesAsync(path);
        var operations = await Fixture.DbContext.StorageOperations.CountAsync();

        using var response = await Fixture.HttpClient.PutAsJsonAsync($"/api/Image/{image.Id}", new UpdateImageDto
        {
            Base64 = "invalid base64!", MimeType = "image/png"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = await Fixture.DbContext.Images.AsNoTracking().SingleAsync(item => item.Id == image.Id);
        Assert.Equal(image.BlobName, saved.BlobName);
        Assert.Equal(image.StorageVersion, saved.StorageVersion);
        Assert.Equal(bytes, await File.ReadAllBytesAsync(path));
        Assert.Equal(operations, await Fixture.DbContext.StorageOperations.CountAsync());
    }

    [Fact]
    public async Task Pdf_CreateReadAndDelete_ShouldCompleteReservationThenProcessDeletion()
    {
        var bytes = "%PDF-1.4"u8.ToArray();
        using var form = new MultipartFormDataContent();
        using var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        form.Add(content, "File", "lifecycle.pdf");
        form.Add(new StringContent("1"), "LanguageId");

        using var create = await Fixture.HttpClient.PostAsync("/api/PdfReports", form);

        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var report = (await create.Content.ReadFromJsonAsync<PdfReportDto>())!;
        var path = Path.Combine(Fixture.PdfEnvironmentVariables.FullPath, report.BlobName);
        Assert.Equal(bytes, await File.ReadAllBytesAsync(path));
        var reservation = await Fixture.DbContext.StorageOperations.AsNoTracking().SingleAsync(item => item.FileName == report.BlobName);
        Assert.Equal(StorageOperationStatus.Completed, reservation.Status);
        using var read = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report.Id}");
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal(bytes, await read.Content.ReadAsByteArrayAsync());

        using var delete = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{report.Id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);
        Assert.True(File.Exists(path));
        var cleanup = await Fixture.DbContext.StorageOperations.AsNoTracking()
            .SingleAsync(item => item.FileName == report.BlobName && item.Kind == StorageOperationKind.Delete);
        Assert.Equal(StorageOperationStatus.Pending, cleanup.Status);
        await Fixture.ProcessPendingStorageOperationsAsync();
        Assert.False(File.Exists(path));
        Assert.False(await Fixture.DbContext.PdfReports.AsNoTracking().AnyAsync(item => item.Id == report.Id));
    }
}
