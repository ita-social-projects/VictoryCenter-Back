using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.TeamCategoryLocalizations.Create;
public class CreateTeamCategoryLocalizationTest : BaseTestClass
{
    public CreateTeamCategoryLocalizationTest(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateTeamCategoryLocalization_ShouldReturnOk()
    {
        var category = await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2) ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createTeamCategoryLocalizationDto = new CreateTeamCategoryLocalizationDto
        {
            EntityId = 4,
            LanguageId = language.Id,
            Name = "Category Name",
            Description = "Category Description"
        };
        var serializedDto = JsonConvert.SerializeObject(createTeamCategoryLocalizationDto);

        var response = await Fixture.HttpClient.PostAsync("/api/TeamCategoryLocalizations/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
    }
}
