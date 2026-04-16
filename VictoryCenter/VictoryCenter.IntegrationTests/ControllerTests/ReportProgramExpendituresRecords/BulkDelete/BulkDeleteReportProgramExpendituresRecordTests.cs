using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportProgramExpendituresRecords.BulkDelete;

public class BulkDeleteReportProgramExpendituresRecordTests : BaseTestClass
{
    public BulkDeleteReportProgramExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    // [Fact]
    // public async Task BulkDelete_ShouldDeleteRecords()
    // {
    //     var category = new HippotherapyProgramCategory
    //     {
    //         Name = "Hippotherapy category",
    //         CreatedAt = DateTimeOffset.UtcNow
    //     };
    //     var category2 = new HippotherapyProgramCategory
    //     {
    //         Name = "Hippotherapy category",
    //         CreatedAt = DateTimeOffset.UtcNow
    //     };
    //     await Fixture.DbContext.HippotherapyProgramCategories.AddAsync(category);
    //     await Fixture.DbContext.SaveChangesAsync();
    //
    //     var record1 = new ReportProgramExpendituresRecord
    //     {
    //         HippotherapyProgramCategoryId = category.Id,
    //         ReportingYear = 2025,
    //         AmountUah = 300m,
    //         AmountUsd = 6m,
    //         CreatedAt = DateTimeOffset.UtcNow
    //     };
    //     var record2 = new ReportProgramExpendituresRecord
    //     {
    //         HippotherapyProgramCategoryId = category2.Id,
    //         ReportingYear = 2026,
    //         AmountUah = 400m,
    //         AmountUsd = 8m,
    //         CreatedAt = DateTimeOffset.UtcNow
    //     };
    //     await Fixture.DbContext.ReportProgramExpendituresRecords.AddRangeAsync(record1, record2);
    //     await Fixture.DbContext.SaveChangesAsync();
    //
    //     var idsToDelete = new List<long> { record1.Id, record2.Id };
    //
    //     HttpResponseMessage response = await Fixture.HttpClient.PostAsJsonAsync(
    //         "/api/ReportProgramExpendituresRecords/bulk-delete", idsToDelete);
    //     response.EnsureSuccessStatusCode();
    //
    //     var remainingRecords = await Fixture.DbContext.ReportProgramExpendituresRecords
    //         .Where(entity => idsToDelete.Contains(entity.Id))
    //         .ToListAsync();
    //
    //     Assert.Empty(remainingRecords);
    // }
    //
    // [Theory]
    // [InlineData(-1, -2)]
    // [InlineData(0, 0)]
    // public async Task BulkDelete_ShouldNotDeleteRecords_NotFound(long id1, long id2)
    // {
    //     var idsToDelete = new List<long> { id1, id2 };
    //
    //     HttpResponseMessage response = await Fixture.HttpClient.PostAsJsonAsync(
    //         "/api/ReportProgramExpendituresRecords/bulk-delete", idsToDelete);
    //
    //     Assert.False(response.IsSuccessStatusCode);
    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
}
