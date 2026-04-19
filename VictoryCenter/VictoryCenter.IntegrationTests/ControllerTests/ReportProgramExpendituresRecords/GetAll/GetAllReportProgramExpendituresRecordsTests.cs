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

        await Fixture.DbContext.ReportProgramExpendituresRecords.AddAsync(new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = category.Id,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ReportProgramExpendituresRecords/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ReportProgramExpendituresRecordDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ReportProgramExpendituresRecordDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFilteredRecords_WhenCategoryIdProvided()
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

        await Fixture.DbContext.ReportProgramExpendituresRecords.AddRangeAsync(
            new ReportProgramExpendituresRecord
            {
                HippotherapyProgramCategoryId = category1.Id,
                ReportingYear = 2025,
                AmountUah = 100m,
                AmountUsd = 5m,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new ReportProgramExpendituresRecord
            {
                HippotherapyProgramCategoryId = category2.Id,
                ReportingYear = 2025,
                AmountUah = 200m,
                AmountUsd = 10m,
                CreatedAt = DateTimeOffset.UtcNow
            });
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync(
            $"/api/ReportProgramExpendituresRecords/?hippotherapyProgramCategoryId={category1.Id}");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ReportProgramExpendituresRecordDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ReportProgramExpendituresRecordDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.All(responseContent, r => Assert.Equal(category1.Id, r.HippotherapyProgramCategoryId));
    }
}
