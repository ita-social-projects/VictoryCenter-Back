using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.GetAll;
public class GetAllPdfReportsTests : BaseTestClass
{
    public GetAllPdfReportsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllPdfReports_ShouldReturnPaginatedList()
    {
        // Arrange
        await SeedPdfReportsAsync(5);

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.Items.Length > 0);
        Assert.True(result.TotalItemsCount > 0);
    }

    [Fact]
    public async Task GetAllPdfReports_ShouldReturnItemsOrderedByPriority()
    {
        // Arrange
        await SeedPdfReportsAsync(3);

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var items = result!.Items;
        for (var i = 1; i < items.Length; i++)
        {
            Assert.True(items[i].Priority >= items[i - 1].Priority);
        }
    }

    [Fact]
    public async Task GetAllPdfReports_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await SeedPdfReportsAsync(5);

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports?Offset=0&Limit=2");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(2, result!.Items.Length);
        Assert.True(result.TotalItemsCount >= 5);
    }

    [Fact]
    public async Task GetAllPdfReports_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        Fixture.DbContext.PdfReports.RemoveRange(Fixture.DbContext.PdfReports);
        await Fixture.DbContext.SaveChangesAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Empty(result!.Items);
        Assert.Equal(0, result.TotalItemsCount);
    }

    private async Task SeedPdfReportsAsync(int count, long languageId = 1)
    {
        var reports = Enumerable.Range(1, count).Select(i => new PdfReport
        {
            Name = $"Звіт {i}",
            BlobName = $"{Guid.NewGuid():N}.pdf",
            FileSizeBytes = 1024 * i,
            Priority = i,
            LanguageId = languageId,
            CreatedAt = DateTimeOffset.UtcNow
        });

        await Fixture.DbContext.PdfReports.AddRangeAsync(reports);
        await Fixture.DbContext.SaveChangesAsync();
    }
}
