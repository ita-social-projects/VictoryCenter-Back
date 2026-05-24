using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.ReportFundsExpendituresSettingsLocalizations.Update;

public class UpdateReportFundsExpendituresSettingsLocalizationTests : BaseTestClass
{
    public UpdateReportFundsExpendituresSettingsLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnOk()
    {
        var localization = await Fixture.DbContext.ReportFundsExpendituresSettingsLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateDto = new UpdateReportFundsExpendituresSettingsLocalizationDto
        {
            DisclaimerTitle = "Updated localized disclaimer text"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresSettingsLocalizations/{localization.EntityId}/{localization.LanguageId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        var updateDto = new UpdateReportFundsExpendituresSettingsLocalizationDto
        {
            DisclaimerTitle = "Updated localized disclaimer text"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            "/api/ReportFundsExpendituresSettingsLocalizations/999999/999999",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnBadRequest_WhenDisclaimerTitleIsInvalid()
    {
        var localization = await Fixture.DbContext.ReportFundsExpendituresSettingsLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateDto = new UpdateReportFundsExpendituresSettingsLocalizationDto
        {
            DisclaimerTitle = ""
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresSettingsLocalizations/{localization.EntityId}/{localization.LanguageId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
