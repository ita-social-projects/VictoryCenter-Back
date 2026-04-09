using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportProgramExpendituresRecords.Create;

public class CreateReportProgramExpendituresRecordTests : BaseTestClass
{
    public CreateReportProgramExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Create_ShouldCreateRecord()
    {
        var programCategory = new ReportProgramExpendituresCategory
        {
            Name = "Some program category",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportProgramExpendituresCategories.AddAsync(programCategory);
        await Fixture.DbContext.SaveChangesAsync();

        var createDto = new CreateReportProgramExpendituresRecordDto
        {
            ProgramCategoryId = programCategory.Id,
            ReportingYear = 2025,
            AmountUah = 400m,
            AmountUsd = 10m
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportProgramExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_ShouldNotCreateRecord_WhenCategoryAlreadyHasRecord()
    {
        var programCategory = new ReportProgramExpendituresCategory
        {
            Name = "Some program category",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportProgramExpendituresCategories.AddAsync(programCategory);
        await Fixture.DbContext.SaveChangesAsync();

        await Fixture.DbContext.ReportProgramExpendituresRecords.AddAsync(new ReportProgramExpendituresRecord
        {
            ProgramCategoryId = programCategory.Id,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();

        var createDto = new CreateReportProgramExpendituresRecordDto
        {
            ProgramCategoryId = programCategory.Id,
            ReportingYear = 2025,
            AmountUah = 500m,
            AmountUsd = 12m
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportProgramExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
