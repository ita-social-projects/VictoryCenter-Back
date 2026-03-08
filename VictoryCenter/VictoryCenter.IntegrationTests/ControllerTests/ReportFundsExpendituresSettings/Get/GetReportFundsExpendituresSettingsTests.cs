using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresSettings.Get;

public class GetReportFundsExpendituresSettingsTests : BaseTestClass
{
    public GetReportFundsExpendituresSettingsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Get_ShouldReturnSettings()
    {
        await EnsureSettingsExistsAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ReportFundsExpendituresSettings/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ReportFundsExpendituresSettingsDto? responseContent =
            JsonConvert.DeserializeObject<ReportFundsExpendituresSettingsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(ReportFundsExpendituresSettingsConstants.SingletonSettingsId, responseContent.Id);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenSettingsMissing()
    {
        var settings = await Fixture.DbContext.ReportFundsExpendituresSettings.ToListAsync();
        Fixture.DbContext.ReportFundsExpendituresSettings.RemoveRange(settings);
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ReportFundsExpendituresSettings/");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
