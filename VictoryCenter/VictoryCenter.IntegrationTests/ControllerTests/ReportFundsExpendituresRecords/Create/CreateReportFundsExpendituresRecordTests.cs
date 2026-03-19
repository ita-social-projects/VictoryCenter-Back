using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresRecords.Create;

public class CreateReportFundsExpendituresRecordTests : BaseTestClass
{
    public CreateReportFundsExpendituresRecordTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Create_ShouldCreateRecord()
    {
        var category = new ReportFundsExpendituresCategory
        {
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.ReportFundsExpendituresCategories.AddAsync(category);
        await Fixture.DbContext.SaveChangesAsync();

        var createDto = new CreateReportFundsExpendituresRecordDto
        {
            CategoryId = category.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 400m,
            AmountUsd = 10m
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_ShouldNotCreateRecord_WhenCategoryAlreadyHasRecord()
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

        var createDto = new CreateReportFundsExpendituresRecordDto
        {
            CategoryId = category.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 500m,
            AmountUsd = 12m
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
