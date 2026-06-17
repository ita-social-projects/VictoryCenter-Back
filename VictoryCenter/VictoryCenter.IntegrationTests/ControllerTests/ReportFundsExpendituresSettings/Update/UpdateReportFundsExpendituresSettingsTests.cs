using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresSettings.Update;

public class UpdateReportFundsExpendituresSettingsTests : BaseTestClass
{
    public UpdateReportFundsExpendituresSettingsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Update_ShouldUpdateSettings()
    {
        await EnsureSettingsExistsAsync();

        var updateDto = new UpdateReportFundsExpendituresSettingsDto
        {
            DisclaimerTitle = "Updated disclaimer",
            ExchangeRate = 41.654321m,
            ProgramExpendituresReportingYear = 2024
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            "/api/ReportFundsExpendituresSettings/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ReportFundsExpendituresSettingsDto? responseContent =
            JsonConvert.DeserializeObject<ReportFundsExpendituresSettingsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateDto.DisclaimerTitle, responseContent.DisclaimerTitle);
        Assert.Equal(updateDto.ExchangeRate, responseContent.ExchangeRate);
        Assert.Equal(updateDto.ProgramExpendituresReportingYear, responseContent.ProgramExpendituresReportingYear);
    }

    [Fact]
    public async Task Update_ShouldNotUpdateSettings_InvalidData()
    {
        await EnsureSettingsExistsAsync();

        var updateDto = new UpdateReportFundsExpendituresSettingsDto
        {
            DisclaimerTitle = "",
            ExchangeRate = 0m
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            "/api/ReportFundsExpendituresSettings/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task EnsureSettingsExistsAsync()
    {
        var existing = await Fixture.DbContext.ReportFundsExpendituresSettings
            .FirstOrDefaultAsync(entity => entity.Id == ReportFundsExpendituresSettingsConstants.SingletonSettingsId);

        if (existing is not null)
        {
            return;
        }

        await Fixture.DbContext.ReportFundsExpendituresSettings.AddAsync(new ReportFundsExpendituresSettingsEntity
        {
            Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
            DisclaimerTitle = "Initial disclaimer",
            ExchangeRate = 40.123456m,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();
    }
}
