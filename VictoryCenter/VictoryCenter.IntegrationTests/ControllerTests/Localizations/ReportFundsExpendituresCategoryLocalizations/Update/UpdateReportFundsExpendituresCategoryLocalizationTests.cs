using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.ReportFundsExpendituresCategoryLocalizations.Update;

public class UpdateReportFundsExpendituresCategoryLocalizationTests : BaseTestClass
{
    public UpdateReportFundsExpendituresCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnOk()
    {
        var localization = await Fixture.DbContext.ReportFundsExpendituresCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateDto = new UpdateReportFundsExpendituresCategoryLocalizationDto
        {
            Name = "Updated Localization Name"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategoryLocalizations/{localization.EntityId}/{localization.LanguageId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        var updateDto = new UpdateReportFundsExpendituresCategoryLocalizationDto
        {
            Name = "Updated Localization Name"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            "/api/ReportFundsExpendituresCategoryLocalizations/999999/999999",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var localization = await Fixture.DbContext.ReportFundsExpendituresCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateDto = new UpdateReportFundsExpendituresCategoryLocalizationDto
        {
            Name = ""
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategoryLocalizations/{localization.EntityId}/{localization.LanguageId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
