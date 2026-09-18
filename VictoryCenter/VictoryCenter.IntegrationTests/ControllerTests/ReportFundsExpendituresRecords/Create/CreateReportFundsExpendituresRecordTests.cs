using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
            Amount = 400m,
            Currency = ReportFundsExpendituresCurrency.Uah
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var settings = await Fixture.DbContext.ReportFundsExpendituresSettings.SingleAsync();
        var createdRecord = JsonConvert.DeserializeObject<ReportFundsExpendituresRecordDto>(
            await response.Content.ReadAsStringAsync())!;

        Assert.Equal(createDto.Amount, createdRecord.AmountUah);
        Assert.Equal(createDto.Amount / settings.ExchangeRate, createdRecord.AmountUsd);
    }

    [Fact]
    public async Task Create_ShouldDeriveAmountUah_WhenAmountIsEnteredInUsd()
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
            Amount = 10m,
            Currency = ReportFundsExpendituresCurrency.Usd
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var settings = await Fixture.DbContext.ReportFundsExpendituresSettings.SingleAsync();
        var createdRecord = JsonConvert.DeserializeObject<ReportFundsExpendituresRecordDto>(
            await response.Content.ReadAsStringAsync())!;

        Assert.Equal(createDto.Amount, createdRecord.AmountUsd);
        Assert.Equal(createDto.Amount * settings.ExchangeRate, createdRecord.AmountUah);
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
            Amount = 500m,
            Currency = ReportFundsExpendituresCurrency.Uah
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresRecords/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
