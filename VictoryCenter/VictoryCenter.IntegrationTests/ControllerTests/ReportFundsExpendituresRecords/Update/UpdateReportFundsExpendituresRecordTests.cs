using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresRecords.Update;

public class UpdateReportFundsExpendituresRecordTests : BaseTestClass
{
    public UpdateReportFundsExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Update_ShouldUpdateRecord()
    {
        var category = new ReportFundsExpendituresCategory
        {
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresCategories.AddAsync(category);
        await Fixture.DbContext.SaveChangesAsync();

        var record = new ReportFundsExpendituresRecord
        {
            CategoryId = category.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 200m,
            AmountUsd = 5m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresRecords.AddAsync(record);
        await Fixture.DbContext.SaveChangesAsync();

        var updateDto = new UpdateReportFundsExpendituresRecordDto
        {
            CategoryId = category.Id,
            AmountUah = 900m,
            AmountUsd = 22m
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresRecords/{record.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Update_ShouldNotUpdateRecord_WhenCategoryAlreadyHasRecord()
    {
        var firstCategory = new ReportFundsExpendituresCategory
        {
            Name = "Income category 1",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var secondCategory = new ReportFundsExpendituresCategory
        {
            Name = "Income category 2",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.ReportFundsExpendituresCategories.AddRangeAsync(firstCategory, secondCategory);
        await Fixture.DbContext.SaveChangesAsync();

        var firstRecord = new ReportFundsExpendituresRecord
        {
            CategoryId = firstCategory.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 200m,
            AmountUsd = 5m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var secondRecord = new ReportFundsExpendituresRecord
        {
            CategoryId = secondCategory.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 7m,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.ReportFundsExpendituresRecords.AddRangeAsync(firstRecord, secondRecord);
        await Fixture.DbContext.SaveChangesAsync();

        var updateDto = new UpdateReportFundsExpendituresRecordDto
        {
            CategoryId = secondCategory.Id,
            AmountUah = 800m,
            AmountUsd = 20m
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresRecords/{firstRecord.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
