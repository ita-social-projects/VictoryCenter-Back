using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportProgramExpendituresRecords.GetSummary;

public class GetReportProgramExpendituresSummaryTests : BaseTestClass
{
    private const string Endpoint = "/api/ReportProgramExpendituresRecords/summary";

    public GetReportProgramExpendituresSummaryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetSummary_ShouldReturnRoundedTotals()
    {
        // Arrange
        await SeedProgramExpendituresRecordsAsync(
            (100.125m, 10.124m),
            (200.235m, 20.235m));

        // Act
        var responseContent = await GetSummaryAsync();

        // Assert
        Assert.Equal(300.36m, responseContent.TotalAmountUah);
        Assert.Equal(30.36m, responseContent.TotalAmountUsd);
    }

    [Fact]
    public async Task GetSummary_ShouldReturnZeroTotals_WhenRecordsDoNotExist()
    {
        // Act
        var responseContent = await GetSummaryAsync();

        // Assert
        Assert.Equal(0m, responseContent.TotalAmountUah);
        Assert.Equal(0m, responseContent.TotalAmountUsd);
    }

    private async Task<ReportProgramExpendituresSummaryDto> GetSummaryAsync()
    {
        var response = await Fixture.HttpClient.GetAsync(Endpoint);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent =
            JsonConvert.DeserializeObject<ReportProgramExpendituresSummaryDto>(responseString);

        Assert.NotNull(responseContent);

        return responseContent!;
    }

    private async Task SeedProgramExpendituresRecordsAsync(params (decimal AmountUah, decimal AmountUsd)[] amounts)
    {
        var categories = amounts
            .Select((_, index) => new HippotherapyProgramCategory
            {
                Name = $"Summary program category {Guid.NewGuid()} {index}",
                CreatedAt = DateTimeOffset.UtcNow
            })
            .ToArray();

        await Fixture.DbContext.HippotherapyProgramCategories.AddRangeAsync(categories);
        await Fixture.DbContext.SaveChangesAsync();

        var records = categories
            .Select((category, index) => new ReportProgramExpendituresRecord
            {
                HippotherapyProgramCategoryId = category.Id,
                ReportingYear = 2025,
                AmountUah = amounts[index].AmountUah,
                AmountUsd = amounts[index].AmountUsd,
                CreatedAt = DateTimeOffset.UtcNow
            })
            .ToArray();

        await Fixture.DbContext.ReportProgramExpendituresRecords.AddRangeAsync(records);
        await Fixture.DbContext.SaveChangesAsync();
    }
}
