using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
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
        // Arrange
        await ClearAndSeedAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/by-language/1");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<PdfReportDto>>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.All(result, dto => Assert.Equal(1, dto.LanguageId));
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_ShouldNotReturnReportsFromOtherLanguage()
    {
        // Arrange
        await ClearAndSeedAsync();

        // Act
        var uaResponse = await Fixture.HttpClient.GetAsync("api/PdfReports/by-language/1");
        var enResponse = await Fixture.HttpClient.GetAsync("api/PdfReports/by-language/2");

        var uaResult = JsonSerializer.Deserialize<List<PdfReportDto>>(
            await uaResponse.Content.ReadAsStringAsync(), JsonOptions);
        var enResult = JsonSerializer.Deserialize<List<PdfReportDto>>(
            await enResponse.Content.ReadAsStringAsync(), JsonOptions);

        // Assert
        Assert.NotNull(uaResult);
        Assert.NotNull(enResult);
        Assert.All(uaResult, dto => Assert.Equal(1, dto.LanguageId));
        Assert.All(enResult, dto => Assert.Equal(2, dto.LanguageId));

        var uaIds = uaResult.Select(x => x.Id).ToHashSet();
        var enIds = enResult.Select(x => x.Id).ToHashSet();
        Assert.Empty(uaIds.Intersect(enIds));
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_ShouldReturnReportsOrderedByPriority()
    {
        // Arrange
        await ClearAndSeedAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/by-language/1");
        var result = JsonSerializer.Deserialize<List<PdfReportDto>>(
            await response.Content.ReadAsStringAsync(), JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        for (var i = 1; i < result.Count; i++)
        {
            Assert.True(result[i].Priority >= result[i - 1].Priority);
        }
    }

    [Fact]
    public async Task GetPdfReportsByLanguage_EmptyForLanguage_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearAndSeedAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("api/PdfReports/by-language/999");
        var result = JsonSerializer.Deserialize<List<PdfReportDto>>(
            await response.Content.ReadAsStringAsync(), JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Empty(result);
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
