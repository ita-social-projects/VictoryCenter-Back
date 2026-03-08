using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresRecords.GetAll;

public class GetAllReportFundsExpendituresRecordsTests : BaseTestClass
{
    public GetAllReportFundsExpendituresRecordsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAll_ShouldReturnRecords()
    {
        var category = new ReportFundsExpendituresCategory
        {
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresCategories.AddAsync(category);
        await Fixture.DbContext.SaveChangesAsync();

        await Fixture.DbContext.ReportFundsExpendituresRecords.AddAsync(new ReportFundsExpendituresRecord
        {
            CategoryId = category.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ReportFundsExpendituresRecords/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ReportFundsExpendituresRecordDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ReportFundsExpendituresRecordDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
