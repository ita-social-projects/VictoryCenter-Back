using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.ReportFundsExpendituresCategoryLocalizations.Create;

public class CreateReportFundsExpendituresCategoryLocalizationTests : BaseTestClass
{
    public CreateReportFundsExpendituresCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnOk()
    {
        var category = await Fixture.DbContext.ReportFundsExpendituresCategories.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 3)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateReportFundsExpendituresCategoryLocalizationDto
        {
            EntityId = category.Id,
            LanguageId = language.Id,
            Name = "New Localization Name"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategoryLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenEntityDoesNotExist()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateReportFundsExpendituresCategoryLocalizationDto
        {
            EntityId = 999999,
            LanguageId = language.Id,
            Name = "Some Localization Name"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategoryLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var category = await Fixture.DbContext.ReportFundsExpendituresCategories.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateReportFundsExpendituresCategoryLocalizationDto
        {
            EntityId = category.Id,
            LanguageId = language.Id,
            Name = ""
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategoryLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
