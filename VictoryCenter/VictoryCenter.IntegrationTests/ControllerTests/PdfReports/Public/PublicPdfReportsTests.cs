using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.Public;

public class PublicPdfReportsTests : BaseTestClass
{
    private static readonly byte[] PdfSignatureBytes =
        [0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34];

    public PublicPdfReportsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPdfReportsByLanguageId_ShouldReturnPaginatedList()
    {
        // Arrange
        await ClearPdfReportsAsync();
        await SeedPdfReportsAsync(languageId: 1, count: 3, priorityBase: 10);

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/1?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Length);
        Assert.Equal(3, result.TotalItemsCount);
    }

    [Fact]
    public async Task GetPdfReportsByLanguageId_ShouldFilterByLanguageId()
    {
        // Arrange
        await ClearPdfReportsAsync();
        await SeedPdfReportsAsync(languageId: 1, count: 2, priorityBase: 10);
        await SeedPdfReportsAsync(languageId: 2, count: 3, priorityBase: 100);

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/1?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Length);
        Assert.Equal(2, result.TotalItemsCount);
        Assert.All(result.Items, item => Assert.Equal((long?)1, item.LanguageId));
    }

    [Fact]
    public async Task GetPdfReportsByLanguageId_NoReportsForLanguage_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearPdfReportsAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/999?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItemsCount);
    }

    [Fact]
    public async Task GetPdfFile_ValidId_ShouldReturnPdfFile()
    {
        // Arrange
        await ClearPdfReportsAsync();
        var report = await CreateReportWithFileAsync("public-pdf-valid", 1);

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/PdfReports/{report.Id}/file");
        var responseBytes = await response.Content.ReadAsByteArrayAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(PdfSignatureBytes, responseBytes);
    }

    [Fact]
    public async Task GetPdfFile_InvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/999999/file");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

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

    private async Task SeedPdfReportsAsync(long languageId, int count, int priorityBase)
    {
        var reports = Enumerable.Range(1, count).Select(i => new PdfReport
        {
            Name = $"Звіт lang{languageId} №{i}",
            BlobName = $"{Guid.NewGuid():N}.pdf",
            FileSizeBytes = 1024 * i,
            Priority = priorityBase + i,
            CreatedAt = DateTimeOffset.UtcNow,
            LanguageId = languageId
        });

        await Fixture.DbContext.PdfReports.AddRangeAsync(reports);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();
    }

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
            CreatedAt = DateTimeOffset.UtcNow,
            LanguageId = 1
        };

        Fixture.DbContext.PdfReports.Add(report);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return report;
    }
}
