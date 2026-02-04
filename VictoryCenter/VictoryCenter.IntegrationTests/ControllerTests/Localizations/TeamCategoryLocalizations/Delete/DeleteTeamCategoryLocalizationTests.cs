using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.TeamCategoryLocalizations.Delete;
public class DeleteTeamCategoryLocalizationTests : BaseTestClass
{
    public DeleteTeamCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteTeamCategoryLocalization_ShouldReturnOk()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long entityId = category.EntityId;
        long languageId = category.LanguageId;

        var response = await Fixture.HttpClient.DeleteAsync($"/api/TeamCategoryLocalizations/{entityId}/{languageId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTeamCategoryLocalization_ShouldReturnNotFound()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long entityId = 5;
        long languageId = category.LanguageId;

        var response = await Fixture.HttpClient.DeleteAsync($"/api/TeamCategoryLocalizations/{entityId}/{languageId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
