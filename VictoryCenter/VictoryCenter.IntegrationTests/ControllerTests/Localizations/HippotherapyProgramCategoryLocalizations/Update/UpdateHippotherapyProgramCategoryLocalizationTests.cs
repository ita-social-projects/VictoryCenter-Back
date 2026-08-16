using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.HippotherapyProgramCategoryLocalizations.Update;

public class UpdateHippotherapyProgramCategoryLocalizationTests : BaseTestClass
{
    public UpdateHippotherapyProgramCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnOk()
    {
        var localization = await Fixture.DbContext.HippotherapyProgramCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateDto = new UpdateHippotherapyProgramCategoryLocalizationDto
        {
            Name = "Updated Loc Name"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/HippotherapyProgramCategoryLocalizations/{localization.EntityId}/{localization.LanguageId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        var updateDto = new UpdateHippotherapyProgramCategoryLocalizationDto
        {
            Name = "Updated Loc Name"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            "/api/HippotherapyProgramCategoryLocalizations/999999/999999",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var localization = await Fixture.DbContext.HippotherapyProgramCategoryLocalizations.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateDto = new UpdateHippotherapyProgramCategoryLocalizationDto
        {
            Name = ""
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/HippotherapyProgramCategoryLocalizations/{localization.EntityId}/{localization.LanguageId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
