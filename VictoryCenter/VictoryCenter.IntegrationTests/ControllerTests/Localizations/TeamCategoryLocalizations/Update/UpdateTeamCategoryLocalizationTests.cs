using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.TeamCategoryLocalizations.Update;
public class UpdateTeamCategoryLocalizationTests : BaseTestClass
{
    public UpdateTeamCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateTeamCategoryLocalization_ShouldReturnOk()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long entityId = category.EntityId;
        long languageId = category.LanguageId;
        var updateTeamCategoryLocalizationDto = new UpdateTeamCategoryLocalizationDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };
        var serializedDto = JsonConvert.SerializeObject(updateTeamCategoryLocalizationDto);

        var response = await Fixture.HttpClient.PutAsync($"/api/TeamCategoryLocalizations/{entityId}/{languageId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTeamCategoryLocalization_ShouldReturnNotFound()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long entityId = 999;
        long languageId = category.LanguageId;
        var updateTeamCategoryLocalizationDto = new UpdateTeamCategoryLocalizationDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };
        var serializedDto = JsonConvert.SerializeObject(updateTeamCategoryLocalizationDto);

        var response = await Fixture.HttpClient.PutAsync($"/api/TeamCategoryLocalizations/{entityId}/{languageId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeamCategoryLocalization_ShouldReturnBadRequest()
    {
        var category = await Fixture.DbContext.TeamCategoryLocalizations.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        long entityId = category.EntityId;
        long languageId = category.LanguageId;
        var updateTeamCategoryLocalizationDto = new UpdateTeamCategoryLocalizationDto
        {
            Name = string.Empty,
            Description = "Updated Description"
        };
        var serializedDto = JsonConvert.SerializeObject(updateTeamCategoryLocalizationDto);

        var response = await Fixture.HttpClient.PutAsync($"/api/TeamCategoryLocalizations/{entityId}/{languageId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
