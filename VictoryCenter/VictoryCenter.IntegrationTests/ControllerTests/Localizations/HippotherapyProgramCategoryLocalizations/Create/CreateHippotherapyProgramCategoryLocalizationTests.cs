using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.HippotherapyProgramCategoryLocalizations.Create;

public class CreateHippotherapyProgramCategoryLocalizationTests : BaseTestClass
{
    public CreateHippotherapyProgramCategoryLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnOk()
    {
        var freshCategory = new HippotherapyProgramCategory
        {
            Name = "Fresh Category For Localization",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await Fixture.DbContext.HippotherapyProgramCategories.AddAsync(freshCategory);
        await Fixture.DbContext.SaveChangesAsync();

        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateHippotherapyProgramCategoryLocalizationDto
        {
            EntityId = freshCategory.Id,
            LanguageId = language.Id,
            Name = "New Loc Name"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/HippotherapyProgramCategoryLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnNotFound_WhenEntityDoesNotExist()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateHippotherapyProgramCategoryLocalizationDto
        {
            EntityId = 999999,
            LanguageId = language.Id,
            Name = "Some Loc Name"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/HippotherapyProgramCategoryLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var category = await Fixture.DbContext.HippotherapyProgramCategories.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        var createDto = new CreateHippotherapyProgramCategoryLocalizationDto
        {
            EntityId = category.Id,
            LanguageId = language.Id,
            Name = ""
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync(
            "/api/HippotherapyProgramCategoryLocalizations/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
