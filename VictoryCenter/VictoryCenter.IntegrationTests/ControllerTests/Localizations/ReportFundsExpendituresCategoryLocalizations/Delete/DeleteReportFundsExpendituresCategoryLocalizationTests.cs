using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.ReportFundsExpendituresCategoryLocalizations.Delete;

public class DeleteReportFundsExpendituresCategoryLocalizationTests : BaseTestClass
{
    public DeleteReportFundsExpendituresCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnOk()
    {
        var localization = await Fixture.DbContext.ReportFundsExpendituresCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportFundsExpendituresCategoryLocalizations/{localization.EntityId}/{localization.LanguageId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        var response = await Fixture.HttpClient.DeleteAsync(
            "/api/ReportFundsExpendituresCategoryLocalizations/999999/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
