using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresRecords.Delete;

public class DeleteReportFundsExpendituresRecordTests : BaseTestClass
{
    public DeleteReportFundsExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Delete_ShouldDeleteRecord()
    {
        var category = new ReportFundsExpendituresCategory
        {
            Name = "Expense category",
            Type = ReportFundsExpendituresType.Expense,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresCategories.AddAsync(category);
        await Fixture.DbContext.SaveChangesAsync();

        var record = new ReportFundsExpendituresRecord
        {
            CategoryId = category.Id,
            Type = ReportFundsExpendituresType.Expense,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 6m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresRecords.AddAsync(record);
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportFundsExpendituresRecords/{record.Id}");
        response.EnsureSuccessStatusCode();

        Assert.Null(await Fixture.DbContext.ReportFundsExpendituresRecords
            .FirstOrDefaultAsync(entity => entity.Id == record.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Delete_ShouldNotDeleteRecord_NotFound(long id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportFundsExpendituresRecords/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
