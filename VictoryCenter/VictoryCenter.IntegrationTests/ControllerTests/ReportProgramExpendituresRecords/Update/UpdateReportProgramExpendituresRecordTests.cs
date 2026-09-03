using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportProgramExpendituresRecords.Update;

public class UpdateReportProgramExpendituresRecordTests : BaseTestClass
{
    public UpdateReportProgramExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Update_ShouldUpdateRecord()
    {
        var programCategory1 = new HippotherapyProgramCategory
        {
            Name = "Category 1",
            CreatedAt = DateTimeOffset.UtcNow
        };
        var programCategory2 = new HippotherapyProgramCategory
        {
            Name = "Category 2",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.HippotherapyProgramCategories.AddRangeAsync(programCategory1, programCategory2);
        await Fixture.DbContext.SaveChangesAsync();

        var record = new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = programCategory1.Id,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportProgramExpendituresRecords.AddAsync(record);
        await Fixture.DbContext.SaveChangesAsync();

        var updateDto = new UpdateReportProgramExpendituresRecordDto
        {
            HippotherapyProgramCategoryId = programCategory2.Id,
            AmountUah = 500m,
            AmountUsd = 10m
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportProgramExpendituresRecords/{record.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var returnedDto = JsonConvert.DeserializeObject<ReportProgramExpendituresRecordDto>(responseContent);

        Assert.NotNull(returnedDto);
        Assert.Equal(record.Id, returnedDto.Id);
        Assert.Equal(updateDto.HippotherapyProgramCategoryId, returnedDto.HippotherapyProgramCategoryId);
        Assert.Equal(updateDto.AmountUah, returnedDto.AmountUah);
        Assert.Equal(updateDto.AmountUsd, returnedDto.AmountUsd);

        var dbRecord = await Fixture.DbContext.ReportProgramExpendituresRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == record.Id);

        Assert.NotNull(dbRecord);
        Assert.Equal(updateDto.HippotherapyProgramCategoryId, dbRecord.HippotherapyProgramCategoryId);
        Assert.Equal(updateDto.AmountUah, dbRecord.AmountUah);
        Assert.Equal(updateDto.AmountUsd, dbRecord.AmountUsd);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenUpdatingToCategoryThatAlreadyHasRecord()
    {
        var programCategory1 = new HippotherapyProgramCategory
        {
            Name = "Category 1 for Conflict Test",
            CreatedAt = DateTimeOffset.UtcNow
        };
        var programCategory2 = new HippotherapyProgramCategory
        {
            Name = "Category 2 for Conflict Test",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.HippotherapyProgramCategories.AddRangeAsync(programCategory1, programCategory2);
        await Fixture.DbContext.SaveChangesAsync();

        var record1 = new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = programCategory1.Id,
            ReportingYear = 2025,
            AmountUah = 300m,
            AmountUsd = 8m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var record2 = new ReportProgramExpendituresRecord
        {
            HippotherapyProgramCategoryId = programCategory2.Id,
            ReportingYear = 2026,
            AmountUah = 400m,
            AmountUsd = 9m,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportProgramExpendituresRecords.AddRangeAsync(record1, record2);
        await Fixture.DbContext.SaveChangesAsync();

        var updateDto = new UpdateReportProgramExpendituresRecordDto
        {
            HippotherapyProgramCategoryId = programCategory2.Id,
            AmountUah = 500m,
            AmountUsd = 12m
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportProgramExpendituresRecords/{record1.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenRecordDoesNotExist()
    {
        var programCategory = new HippotherapyProgramCategory
        {
            Name = "Category for missing record test",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.HippotherapyProgramCategories.AddAsync(programCategory);
        await Fixture.DbContext.SaveChangesAsync();
        var updateDto = new UpdateReportProgramExpendituresRecordDto
        {
            HippotherapyProgramCategoryId = programCategory.Id,
            AmountUah = 500m,
            AmountUsd = 12m
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);
        var response = await Fixture.HttpClient.PutAsync(
            "/api/ReportProgramExpendituresRecords/9999",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
