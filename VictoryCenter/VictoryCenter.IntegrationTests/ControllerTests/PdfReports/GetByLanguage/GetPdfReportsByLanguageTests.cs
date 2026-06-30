using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfReports.GetByLanguage;

public class GetPdfReportsByLanguageTests : BaseTestClass
{
    public GetPdfReportsByLanguageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_ShouldReturnOnlyReportsForRequestedLanguage()
    {
        await ClearAndSeedAsync();

        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/1?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);
        Assert.NotNull(result);
        Assert.All(result.Items, dto => Assert.Equal(1, dto.LanguageId));
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_ShouldNotReturnReportsFromOtherLanguage()
    {
        await ClearAndSeedAsync();

        var uaResponse = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/1?Offset=0&Limit=20");
        var enResponse = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/2?Offset=0&Limit=20");

        Assert.True(uaResponse.IsSuccessStatusCode);
        Assert.True(enResponse.IsSuccessStatusCode);

        var uaResult = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(
            await uaResponse.Content.ReadAsStringAsync(), JsonOptions);
        var enResult = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(
            await enResponse.Content.ReadAsStringAsync(), JsonOptions);

        Assert.NotNull(uaResult);
        Assert.NotNull(enResult);
        Assert.All(uaResult.Items, dto => Assert.Equal(1, dto.LanguageId));
        Assert.All(enResult.Items, dto => Assert.Equal(2, dto.LanguageId));

        var uaIds = uaResult.Items.Select(x => x.Id).ToHashSet();
        var enIds = enResult.Items.Select(x => x.Id).ToHashSet();
        Assert.Empty(uaIds.Intersect(enIds));
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_ShouldReturnReportsOrderedByPriority()
    {
        await ClearAndSeedAsync();

        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/1?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);
        Assert.NotNull(result);
        for (var i = 1; i < result.Items.Length; i++)
        {
            Assert.True(result.Items[i].Priority >= result.Items[i - 1].Priority);
        }
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_EmptyForLanguage_ShouldReturnEmptyList()
    {
        await ClearAndSeedAsync();

        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/languageId/999?Offset=0&Limit=20");
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        var result = JsonSerializer.Deserialize<PaginationResult<PdfReportDto>>(responseString, JsonOptions);
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    private async Task ClearAndSeedAsync()
    {
        Fixture.DbContext.PdfReports.RemoveRange(Fixture.DbContext.PdfReports);
        await Fixture.DbContext.SaveChangesAsync();

        var reports = new List<PdfReport>
        {
            new() { Name = "UA Звіт 1", BlobName = $"{Guid.NewGuid():N}.pdf", FileSizeBytes = 1024, Priority = 1, LanguageId = 1, CreatedAt = DateTimeOffset.UtcNow },
            new() { Name = "UA Звіт 2", BlobName = $"{Guid.NewGuid():N}.pdf", FileSizeBytes = 2048, Priority = 2, LanguageId = 1, CreatedAt = DateTimeOffset.UtcNow },
            new() { Name = "EN Report 1", BlobName = $"{Guid.NewGuid():N}.pdf", FileSizeBytes = 1024, Priority = 1, LanguageId = 2, CreatedAt = DateTimeOffset.UtcNow },
        };

        await Fixture.DbContext.PdfReports.AddRangeAsync(reports);
        await Fixture.DbContext.SaveChangesAsync();
    }
}
