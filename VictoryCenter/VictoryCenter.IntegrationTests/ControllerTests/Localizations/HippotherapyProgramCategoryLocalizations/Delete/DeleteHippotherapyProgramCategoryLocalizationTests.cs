using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.HippotherapyProgramCategoryLocalizations.Delete;

public class DeleteHippotherapyProgramCategoryLocalizationTests : BaseTestClass
{
    public DeleteHippotherapyProgramCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnOk()
    {
        var localization = await Fixture.DbContext.HippotherapyProgramCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var response = await Fixture.HttpClient.DeleteAsync(
            $"/api/HippotherapyProgramCategoryLocalizations/{localization.EntityId}/{localization.LanguageId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        var response = await Fixture.HttpClient.DeleteAsync(
            "/api/HippotherapyProgramCategoryLocalizations/999999/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
