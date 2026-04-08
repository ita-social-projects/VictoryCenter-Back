using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.GetById;

public class GetPdfReportByIdTests : BaseTestClass
{
    private static readonly byte[] PdfSignatureBytes =
        [0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34];

    public GetPdfReportByIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPdfReportById_ValidId_ShouldReturnOk()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("get-valid-test", 1);

        // Act
        var response = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPdfReportById_ValidId_ShouldReturnPdfContentType()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("get-content-type-test", 1);

        // Act
        var response = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetPdfReportById_ValidId_ShouldReturnCorrectFileBytes()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("get-bytes-test", 1);

        // Act
        var response = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report.Id}");
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(PdfSignatureBytes, responseBytes);
    }

    [Fact]
    public async Task GetPdfReportById_ValidId_ShouldNotAlterDatabaseRecord()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("get-no-alter-test", 1);
        var countBefore = await Fixture.DbContext.PdfReports.CountAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var countAfter = await Fixture.DbContext.PdfReports.CountAsync();
        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task GetPdfReportById_InvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await Fixture.HttpClient.GetAsync("/api/PdfReports/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPdfReportById_ValidId_ResponseShouldNotBeEmpty()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("get-not-empty-test", 1);

        // Act
        var response = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report.Id}");
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBytes);
    }

    [Fact]
    public async Task GetPdfReportById_MultipleReports_ShouldReturnCorrectOne()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var bytes1 = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34, 0x01 };
        var bytes2 = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34, 0x02 };
        var report1 = await CreateReportWithFileAsync("multi-get-1", 1, bytes1);
        var report2 = await CreateReportWithFileAsync("multi-get-2", 2, bytes2);

        // Act
        var response = await Fixture.HttpClient.GetAsync($"/api/PdfReports/{report2.Id}");
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(bytes2, responseBytes);

        Fixture.DbContext.ChangeTracker.Clear();
        var fetchedReport = await Fixture.DbContext.PdfReports
            .FirstOrDefaultAsync(r => r.Id == report2.Id);

        Assert.NotNull(fetchedReport);
        Assert.Equal(report2.BlobName, fetchedReport.BlobName);
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

        foreach (var entry in Fixture.DbContext.ChangeTracker.Entries()
        .Where(e => !all.Contains(e.Entity))
        .ToList())
        {
            entry.State = EntityState.Detached;
        }

        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Writes a real PDF file to disk and inserts a matching DB record.
    /// BlobName is always "{baseName}.pdf" — exactly what PdfService produces.
    /// </summary>
    private async Task<PdfReport> CreateReportWithFileAsync(
        string baseName,
        int priority,
        byte[] fileBytes = null!)
    {
        var blobName = $"{baseName}.pdf";
        var filePath = Path.Combine(Fixture.PdfEnvironmentVariables.FullPath, blobName);

        Directory.CreateDirectory(Fixture.PdfEnvironmentVariables.FullPath);

        var content = fileBytes ?? PdfSignatureBytes;
        await File.WriteAllBytesAsync(filePath, content);

        var report = new PdfReport
        {
            Name = baseName,
            BlobName = blobName,
            FileSizeBytes = content.Length,
            Priority = priority,
            CreatedAt = DateTimeOffset.UtcNow
        };

        Fixture.DbContext.PdfReports.Add(report);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return report;
    }
}
