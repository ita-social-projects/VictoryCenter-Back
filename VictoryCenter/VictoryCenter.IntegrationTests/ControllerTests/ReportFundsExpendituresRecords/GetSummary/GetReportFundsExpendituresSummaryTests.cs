using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresRecords.GetSummary;

public class GetReportFundsExpendituresSummaryTests : BaseTestClass
{
    public GetReportFundsExpendituresSummaryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetSummary_ShouldReturnSummary()
    {
        var incomeCategory = new ReportFundsExpendituresCategory
        {
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var expenseCategory = new ReportFundsExpendituresCategory
        {
            Name = "Expense category",
            Type = ReportFundsExpendituresType.Expense,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.ReportFundsExpendituresCategories.AddRangeAsync(incomeCategory, expenseCategory);
        await Fixture.DbContext.SaveChangesAsync();

        await Fixture.DbContext.ReportFundsExpendituresRecords.AddRangeAsync(
            new ReportFundsExpendituresRecord
            {
                CategoryId = incomeCategory.Id,
                Type = ReportFundsExpendituresType.Income,
                ReportingYear = 2025,
                AmountUah = 1000.45m,
                AmountUsd = 20.49m,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new ReportFundsExpendituresRecord
            {
                CategoryId = expenseCategory.Id,
                Type = ReportFundsExpendituresType.Expense,
                ReportingYear = 2025,
                AmountUah = 500.5m,
                AmountUsd = 10.5m,
                CreatedAt = DateTimeOffset.UtcNow
            });
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ReportFundsExpendituresRecords/summary");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ReportFundsExpendituresSummaryDto? responseContent =
            JsonConvert.DeserializeObject<ReportFundsExpendituresSummaryDto>(responseString);

        Assert.NotNull(responseContent);

        Assert.Equal(1000m, responseContent.IncomeUahTotal);

        Assert.Equal(20m, responseContent.IncomeUsdTotal);

        Assert.Equal(501m, responseContent.ExpenditureUahTotal);

        Assert.Equal(11m, responseContent.ExpenditureUsdTotal);
    }
}
