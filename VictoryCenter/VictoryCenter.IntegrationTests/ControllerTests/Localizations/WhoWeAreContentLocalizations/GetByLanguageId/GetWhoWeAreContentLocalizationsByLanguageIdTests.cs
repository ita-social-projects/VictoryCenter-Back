using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.WhoWeAreContentLocalizations.GetByLanguageId;

public class GetWhoWeAreContentLocalizationsByLanguageIdTests : BaseTestClass
{
    public GetWhoWeAreContentLocalizationsByLanguageIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetWhoWeAreContentLocalizationsByLanguageId_ShouldReturnOk()
    {
        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var response = await Fixture.HttpClient
            .GetAsync($"/api/WhoWeAreContentLocalizations/languageId/{language.Id}");

        Assert.True(response.IsSuccessStatusCode);
    }
}
