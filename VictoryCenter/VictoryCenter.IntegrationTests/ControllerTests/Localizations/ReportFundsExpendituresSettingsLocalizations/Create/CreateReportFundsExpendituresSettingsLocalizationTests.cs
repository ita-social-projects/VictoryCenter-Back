using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.ReportFundsExpendituresSettingsLocalizations.Create;

public class CreateReportFundsExpendituresSettingsLocalizationTests : BaseTestClass
{
    public CreateReportFundsExpendituresSettingsLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnOk()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var existingLocalization = await Fixture.DbContext.ReportFundsExpendituresSettingsLocalizations
            .FirstOrDefaultAsync(l =>
                l.EntityId == ReportFundsExpendituresSettingsConstants.SingletonSettingsId &&
                l.LanguageId == language.Id);

        if (existingLocalization is not null)
        {
            Fixture.DbContext.ReportFundsExpendituresSettingsLocalizations.Remove(existingLocalization);
            await Fixture.DbContext.SaveChangesAsync();
        }

        var createDto = new CreateReportFundsExpendituresSettingsLocalizationDto
        {
            EntityId = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
            LanguageId = language.Id,
            DisclaimerTitle = "New localized disclaimer text"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresSettingsLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenEntityDoesNotExist()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateReportFundsExpendituresSettingsLocalizationDto
        {
            EntityId = 999999,
            LanguageId = language.Id,
            DisclaimerTitle = "Some disclaimer text"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresSettingsLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnBadRequest_WhenDisclaimerTitleIsInvalid()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateReportFundsExpendituresSettingsLocalizationDto
        {
            EntityId = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
            LanguageId = language.Id,
            DisclaimerTitle = ""
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresSettingsLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
