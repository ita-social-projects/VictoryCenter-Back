using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresRecords.BulkDelete;

public class BulkDeleteReportFundsExpendituresRecordTests : BaseTestClass
{
    public BulkDeleteReportFundsExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task BulkDelete_ShouldDeleteRecords()
    {
        var category = new ReportFundsExpendituresCategory
        {
            Name = "Category",
            Type = VictoryCenter.DAL.Enums.ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var category2 = new ReportFundsExpendituresCategory
        {
            Name = "Category2",
            Type = VictoryCenter.DAL.Enums.ReportFundsExpendituresType.Expense,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresCategories.AddRangeAsync(category, category2);
        await Fixture.DbContext.SaveChangesAsync();

        var record1 = new ReportFundsExpendituresRecord
        {
            CategoryId = category.Id,
            Type = VictoryCenter.DAL.Enums.ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 6m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var record2 = new ReportFundsExpendituresRecord
        {
            CategoryId = category2.Id,
            Type = VictoryCenter.DAL.Enums.ReportFundsExpendituresType.Expense,
            ReportingYear = 2026,
            AmountUah = 400m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresRecords.AddRangeAsync(record1, record2);
        await Fixture.DbContext.SaveChangesAsync();

        var idsToDelete = new List<long> { record1.Id, record2.Id };
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/ReportFundsExpendituresRecords/bulk-delete", idsToDelete);
        response.EnsureSuccessStatusCode();

        var remainingRecords = await Fixture.DbContext.ReportFundsExpendituresRecords
            .Where(entity => idsToDelete.Contains(entity.Id))
            .ToListAsync();

        Assert.Empty(remainingRecords);
    }

    [Theory]
    [InlineData(1111, 22222)]
    [InlineData(2131231, 3424324)]
    public async Task BulkDelete_ShouldNotDeleteRecords_NotFound(long id1, long id2)
    {
        var idsToDelete = new List<long> { id1, id2 };

        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/ReportFundsExpendituresRecords/bulk-delete", idsToDelete);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
