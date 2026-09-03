using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

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
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
        ?? throw new InvalidOperationException("Couldn't setup existing language");

        var localization = await EnsureLocalizationExistsAsync(language.Id);

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
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
        ?? throw new InvalidOperationException("Couldn't setup existing language");

        var localization = await EnsureLocalizationExistsAsync(language.Id);

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

    [Fact]
    public async Task UpdateLocalization_ShouldReturnBadRequest_WhenDisclaimerTitleIsTooShortAfterTrimming()
    {
       var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
        ?? throw new InvalidOperationException("Couldn't setup existing language");

       var localization = await EnsureLocalizationExistsAsync(language.Id);

       var updateDto = new UpdateReportFundsExpendituresSettingsLocalizationDto
    {
        DisclaimerTitle = "  A  ",
    };
       var serializedDto = JsonConvert.SerializeObject(updateDto);

       HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
        $"/api/ReportFundsExpendituresSettingsLocalizations/{localization.EntityId}/{localization.LanguageId}",
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

    private async Task<ReportFundsExpendituresSettingsLocalization> EnsureLocalizationExistsAsync(long languageId)
{
    await EnsureSettingsExistsAsync();

    var existing = await Fixture.DbContext.ReportFundsExpendituresSettingsLocalizations
        .FirstOrDefaultAsync(l =>
            l.EntityId == ReportFundsExpendituresSettingsConstants.SingletonSettingsId &&
            l.LanguageId == languageId);

    if (existing is not null)
    {
        return existing;
    }

    var entity = new ReportFundsExpendituresSettingsLocalization
    {
        EntityId = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
        LanguageId = languageId,
        DisclaimerTitle = "Initial localized disclaimer",
        TranslationStatus = TranslationStatus.Relevant,
        CreatedAt = DateTimeOffset.UtcNow
    };

    await Fixture.DbContext.ReportFundsExpendituresSettingsLocalizations.AddAsync(entity);
    await Fixture.DbContext.SaveChangesAsync();

    return entity;
}
}
