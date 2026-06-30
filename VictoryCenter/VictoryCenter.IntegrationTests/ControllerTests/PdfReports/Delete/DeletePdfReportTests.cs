using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.Delete;

public class DeletePdfReportTests : BaseTestClass
{
    private static readonly byte[] PdfSignatureBytes =
        [0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34];

    public DeletePdfReportTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeletePdfReport_ValidRequest_ShouldDeleteAndReturnOk()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("delete-valid-test", 1);
        var countBefore = await Fixture.DbContext.PdfReports.CountAsync();

        // Act
        var response = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{report.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var countAfter = await Fixture.DbContext.PdfReports.CountAsync();
        Assert.Equal(countBefore - 1, countAfter);
    }

    [Fact]
    public async Task DeletePdfReport_ShouldRemoveFileFromDisk()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("delete-file-cleanup", 1);

        var filePath = Path.Combine(Fixture.PdfEnvironmentVariables.FullPath, report.BlobName);
        Assert.True(File.Exists(filePath), $"File should exist before delete: {filePath}");

        // Act
        var response = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{report.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(File.Exists(filePath), $"File should be removed after delete: {filePath}");
    }

    [Fact]
    public async Task DeletePdfReport_AfterDelete_ReportShouldNotExistInDatabase()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("delete-db-check", 1);
        var savedId = report.Id;

        // Act
        var response = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{savedId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var stillExists = await Fixture.DbContext.PdfReports.AnyAsync(r => r.Id == savedId);
        Assert.False(stillExists);
    }

    [Fact]
    public async Task DeletePdfReport_InvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await Fixture.HttpClient.DeleteAsync("/api/PdfReports/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePdfReport_ShouldReorderRemainingPriorities()
    {
        // Arrange
        await ClearPdfReportsAsync();

        var report1 = await CreateReportWithFileAsync("reorder-1", 1);
        var report2 = await CreateReportWithFileAsync("reorder-2", 2);
        var report3 = await CreateReportWithFileAsync("reorder-3", 3);

        // Act
        var response = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{report1.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();

        var deletedStillExists = await Fixture.DbContext.PdfReports.AnyAsync(r => r.Id == report1.Id);
        Assert.False(deletedStillExists, "Deleted report should not exist in database");

        var remaining = await Fixture.DbContext.PdfReports
            .OrderBy(r => r.Priority)
            .ToListAsync();

        Assert.Equal(2, remaining.Count);
        Assert.Equal(report2.Id, remaining[0].Id);
        Assert.Equal(report3.Id, remaining[1].Id);

        for (var i = 0; i < remaining.Count; i++)
        {
            Assert.Equal(i + 1, remaining[i].Priority);
        }
    }

    [Fact]
    public async Task DeletePdfReport_DeleteMiddleReport_ShouldReorderCorrectly()
    {
        // Arrange
        await ClearPdfReportsAsync();

        var report1 = await CreateReportWithFileAsync("middle-1", 1);
        var report2 = await CreateReportWithFileAsync("middle-2", 2);
        var report3 = await CreateReportWithFileAsync("middle-3", 3);

        // Act
        var response = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{report2.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();

        var deletedStillExists = await Fixture.DbContext.PdfReports.AnyAsync(r => r.Id == report2.Id);
        Assert.False(deletedStillExists, "Deleted report should not exist in database");

        var remaining = await Fixture.DbContext.PdfReports
            .OrderBy(r => r.Priority)
            .ToListAsync();

        Assert.Equal(2, remaining.Count);
        Assert.Equal(report1.Id, remaining[0].Id);
        Assert.Equal(report3.Id, remaining[1].Id);
        Assert.Equal(1, remaining[0].Priority);
        Assert.Equal(2, remaining[1].Priority);
    }

    [Fact]
    public async Task DeletePdfReport_OnlyOneReport_ShouldDeleteSuccessfully()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("single-report", 1);

        // Act
        var response = await Fixture.HttpClient.DeleteAsync($"/api/PdfReports/{report.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        Assert.Equal(0, await Fixture.DbContext.PdfReports.CountAsync());
    }

    /// <summary>
    /// Removes all PdfReport rows and their files so each test starts clean.
    /// </summary>
    private async Task ClearPdfReportsAsync()
    {
        var all = await Fixture.DbContext.PdfReports.ToListAsync();

        foreach (var r in all)
        {
            var fp = Path.Combine(Fixture.PdfEnvironmentVariables.FullPath, r.BlobName);
            if (File.Exists(fp))
            {
                File.Delete(fp);
            }
        }

        Fixture.DbContext.PdfReports.RemoveRange(all);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Writes a real PDF file to disk and inserts a matching DB record.
    /// BlobName is always "{baseName}.pdf" — exactly what PdfService produces.
    /// </summary>
    private async Task<PdfReport> CreateReportWithFileAsync(string baseName, int priority)
    {
        var blobName = $"{baseName}.pdf";
        var filePath = Path.Combine(Fixture.PdfEnvironmentVariables.FullPath, blobName);

        Directory.CreateDirectory(Fixture.PdfEnvironmentVariables.FullPath);

        await File.WriteAllBytesAsync(filePath, PdfSignatureBytes);

        var report = new PdfReport
        {
            Name = baseName,
            BlobName = blobName,
            FileSizeBytes = PdfSignatureBytes.Length,
            Priority = priority,
            LanguageId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        Fixture.DbContext.PdfReports.Add(report);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return report;
    }
}
