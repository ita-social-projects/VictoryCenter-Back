using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.WhoWeAreContentLocalizations.GetByEntityId;

public class GetWhoWeAreContentLocalizationsByEntityIdTests : BaseTestClass
{
    public GetWhoWeAreContentLocalizationsByEntityIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetWhoWeAreContentLocalizationsByEntityId_ShouldReturnOk()
    {
        var content = await Fixture.DbContext.WhoWeAreContents
            .FirstOrDefaultAsync(c => c.ContentType == ContentType.Title)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var response = await Fixture.HttpClient
            .GetAsync($"/api/WhoWeAreContentLocalizations/entityId/{content.Id}");

        Assert.True(response.IsSuccessStatusCode);
    }
}
