using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportProgramExpendituresRecords.GetAll;

public class GetAllReportProgramExpendituresRecordsTests : BaseTestClass
{
    public GetAllReportProgramExpendituresRecordsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAll_ShouldReturnRecords()
    {
        var category = new HippotherapyProgramCategory
        {
            Name = "Some program category",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.HippotherapyProgramCategories.AddAsync(category);
        await Fixture.DbContext.SaveChangesAsync();

        var record = new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = category.Id,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportProgramExpendituresRecords.AddAsync(record);
        await Fixture.DbContext.SaveChangesAsync();

        var response = await Fixture.HttpClient.GetAsync("/api/ReportProgramExpendituresRecords/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ReportProgramExpendituresRecordDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.Contains(responseContent, r => r.Id == record.Id);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFilteredRecords_WhenCategoryIdsProvided()
    {
        var category1 = new HippotherapyProgramCategory
        {
            Name = "Program category 1",
            CreatedAt = DateTimeOffset.UtcNow
        };
        var category2 = new HippotherapyProgramCategory
        {
            Name = "Program category 2",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.HippotherapyProgramCategories.AddRangeAsync(category1, category2);
        await Fixture.DbContext.SaveChangesAsync();

        var record1 = new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = category1.Id,
            ReportingYear = 2025,
            AmountUah = 100m,
            AmountUsd = 5m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var record2 = new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = category2.Id,
            ReportingYear = 2025,
            AmountUah = 200m,
            AmountUsd = 10m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportProgramExpendituresRecords.AddRangeAsync(record1, record2);
        await Fixture.DbContext.SaveChangesAsync();

        var response = await Fixture.HttpClient.GetAsync(
            $"/api/ReportProgramExpendituresRecords/?hippotherapyProgramCategoryIds={category1.Id}");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ReportProgramExpendituresRecordDto>>(responseString);

        Assert.NotNull(responseContent);
        var reportProgramExpendituresRecordDtos =
            responseContent as ReportProgramExpendituresRecordDto[] ?? responseContent.ToArray();
        Assert.Contains(reportProgramExpendituresRecordDtos, r => r.Id == record1.Id);
        Assert.DoesNotContain(reportProgramExpendituresRecordDtos, r => r.Id == record2.Id);
    }
}
