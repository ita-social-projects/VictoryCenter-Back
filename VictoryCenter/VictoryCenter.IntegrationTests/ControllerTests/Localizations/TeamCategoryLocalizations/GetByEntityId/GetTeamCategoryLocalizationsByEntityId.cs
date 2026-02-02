using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.TeamCategoryLocalizations.GetByEntityId;
public class GetTeamCategoryLocalizationsByEntityId : BaseTestClass
{
    public GetTeamCategoryLocalizationsByEntityId(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetTeamCategoryLocalizationsByEntityId_ShouldReturnOk()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync(l => l.EntityId == 1) ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long entityId = category.EntityId;

        var response = await Fixture.HttpClient.GetAsync($"/api/TeamCategoryLocalizations/entityId/{entityId}");

        Assert.True(response.IsSuccessStatusCode);
    }
}
