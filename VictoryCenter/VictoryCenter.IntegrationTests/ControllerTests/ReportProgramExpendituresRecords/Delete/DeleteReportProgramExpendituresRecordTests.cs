using System.Net;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportProgramExpendituresRecords.Delete;

public class DeleteReportProgramExpendituresRecordTests : BaseTestClass
{
    private ReportProgramExpendituresRecord _validReportProgramExpendituresRecord = null!;

    public DeleteReportProgramExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _validReportProgramExpendituresRecord = await CreateValidReportProgramExpendituresRecordInDatabaseAsync();
    }

    [Fact]
    public async Task Delete_ShouldDeleteRecord_WhenRecordExists()
    {
        var response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportProgramExpendituresRecords/{_validReportProgramExpendituresRecord.Id}");

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentToLongConversionResult = long.TryParse(responseContent, out var responseId);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseContentToLongConversionResult);
        Assert.Equal(_validReportProgramExpendituresRecord.Id, responseId);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenRecordDoesNotExist()
    {
        const long invalidId = 99999;
        var response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportProgramExpendituresRecords/{invalidId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ReportProgramExpendituresRecord> CreateValidReportProgramExpendituresRecordInDatabaseAsync()
    {
        var programCategory = new HippotherapyProgramCategory
        {
            Name = "Some program category",
            CreatedAt = DateTimeOffset.UtcNow
        };
        Fixture.DbContext.HippotherapyProgramCategories.Add(programCategory);

        var record = new ReportProgramExpendituresRecord
        {
            CreatedAt = DateTimeOffset.UtcNow,
            ReportingYear = 2016,
            AmountUah = 100,
            AmountUsd = 100,
            HippotherapyProgramCategory = programCategory
        };
        Fixture.DbContext.ReportProgramExpendituresRecords.Add(record);

        await Fixture.DbContext.SaveChangesAsync();

        return record;
    }
}
