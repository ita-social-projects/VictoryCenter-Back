using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.TeamCategoryLocalizations.GetByLanguageId;
public class GetTeamCategoryLocalizationsByLanguageIdTests : BaseTestClass
{
    public GetTeamCategoryLocalizationsByLanguageIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetTeamCategoryLocalizationsByLanguageId_ShouldReturnOk()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync(l => l.LanguageId == 2) ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long languageId = category.LanguageId;

        var response = await Fixture.HttpClient.GetAsync($"/api/TeamCategoryLocalizations/languageId/{languageId}");

        Assert.True(response.IsSuccessStatusCode);
    }
}
