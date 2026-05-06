using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.ReportFundsExpendituresCategoryLocalizations.GetByEntityId;

public class GetLocalizationsByEntityIdTests : BaseTestClass
{
    public GetLocalizationsByEntityIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetLocalizationsByEntityId_ShouldReturnOk_WhenEntityExists()
    {
        var localization = await Fixture.DbContext.ReportFundsExpendituresCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var response = await Fixture.HttpClient.GetAsync(
            $"/api/ReportFundsExpendituresCategoryLocalizations/{localization.EntityId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLocalizationsByEntityId_ShouldReturnOk_WithEmptyList_WhenEntityHasNoLocalizations()
    {
        var response = await Fixture.HttpClient.GetAsync(
            "/api/ReportFundsExpendituresCategoryLocalizations/999999");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
