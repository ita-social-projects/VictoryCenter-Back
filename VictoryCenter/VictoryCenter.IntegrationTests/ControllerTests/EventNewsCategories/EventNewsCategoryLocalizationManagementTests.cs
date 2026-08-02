using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNewsCategories;

public class EventNewsCategoryLocalizationManagementTests : BaseTestClass
{
    public EventNewsCategoryLocalizationManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task LocalizationManagement_ShouldSupportAuthorizedCrud()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var categoryName = $"Cat{suffix}";
        var localizedName = $"Loc{suffix}";
        var updatedLocalizedName = $"Upd{suffix}";
        var renamedCategory = $"Ren{suffix}";
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();

        var createCategoryResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategories",
            new CreateEventNewsCategoryDto { Name = categoryName });
        createCategoryResponse.EnsureSuccessStatusCode();
        var category = await createCategoryResponse.Content.ReadFromJsonAsync<AdminEventNewsCategoryDto>();
        Assert.NotNull(category);

        var createLocalizationResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = category.Id,
                LanguageId = language.Id,
                Name = localizedName
            });
        Assert.Equal(HttpStatusCode.OK, createLocalizationResponse.StatusCode);
        var localization = await createLocalizationResponse.Content
            .ReadFromJsonAsync<AdminEventNewsCategoryLocalizationDto>();
        Assert.NotNull(localization);
        Assert.Equal(language.Id, localization.Language.Id);
        Assert.Equal(language.Code, localization.Language.Code);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);

        var updateLocalizationResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/EventNewsCategoryLocalizations/{category.Id}/{language.Id}",
            new UpdateEventNewsCategoryLocalizationDto { Name = updatedLocalizedName });
        Assert.Equal(HttpStatusCode.OK, updateLocalizationResponse.StatusCode);

        var updateCategoryResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/EventNewsCategories/{category.Id}",
            new UpdateEventNewsCategoryDto { Name = renamedCategory });
        Assert.Equal(HttpStatusCode.OK, updateCategoryResponse.StatusCode);

        var getLocalizationsResponse = await Fixture.HttpClient.GetAsync(
            $"/api/EventNewsCategoryLocalizations/entityId/{category.Id}");
        Assert.Equal(HttpStatusCode.OK, getLocalizationsResponse.StatusCode);
        var localizations = await getLocalizationsResponse.Content
            .ReadFromJsonAsync<List<AdminEventNewsCategoryLocalizationDto>>();
        var updatedLocalization = Assert.Single(localizations!);
        Assert.Equal(updatedLocalizedName, updatedLocalization.Name);
        Assert.Equal(TranslationStatus.Outdated, updatedLocalization.TranslationStatus);

        var categories = await Fixture.HttpClient.GetFromJsonAsync<List<AdminEventNewsCategoryDto>>(
            "/api/EventNewsCategories");
        var categoryWithLocalization = Assert.Single(categories!, item => item.Id == category.Id);
        Assert.Single(categoryWithLocalization.Localizations);

        var deleteLocalizationResponse = await Fixture.HttpClient.DeleteAsync(
            $"/api/EventNewsCategoryLocalizations/{category.Id}/{language.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteLocalizationResponse.StatusCode);

        var deleteCategoryResponse = await Fixture.HttpClient.DeleteAsync(
            $"/api/EventNewsCategories/{category.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteCategoryResponse.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldRejectDuplicateEntityLanguagePairAndLocalizedName()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();
        var firstCategory = await CreateCategoryAsync($"One{suffix}");
        var secondCategory = await CreateCategoryAsync($"Two{suffix}");
        var localizedName = $"Loc{suffix}";
        var localization = new CreateEventNewsCategoryLocalizationDto
        {
            EntityId = firstCategory.Id,
            LanguageId = language.Id,
            Name = localizedName
        };

        var firstResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            localization);
        var duplicatePairResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            localization);
        var duplicateNameResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            localization with { EntityId = secondCategory.Id });

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicatePairResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateNameResponse.StatusCode);
        Assert.Contains(
            EventNewsCategoryConstants.LocalizationAlreadyExists,
            await duplicatePairResponse.Content.ReadAsStringAsync());
        Assert.Contains(
            EventNewsCategoryConstants.DuplicateLocalizedName,
            await duplicateNameResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnNotFoundForMissingResources()
    {
        const long missingId = long.MaxValue;
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();
        var category = await CreateCategoryAsync($"Cat{Guid.NewGuid():N}"[..11]);

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = missingId,
                LanguageId = language.Id,
                Name = "Missing"
            });
        var missingLanguageResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = category.Id,
                LanguageId = missingId,
                Name = "Missing"
            });
        var getResponse = await Fixture.HttpClient.GetAsync(
            $"/api/EventNewsCategoryLocalizations/entityId/{missingId}");
        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/EventNewsCategoryLocalizations/{missingId}/{language.Id}",
            new UpdateEventNewsCategoryLocalizationDto { Name = "Missing" });
        var deleteResponse = await Fixture.HttpClient.DeleteAsync(
            $"/api/EventNewsCategoryLocalizations/{missingId}/{language.Id}");

        Assert.Equal(HttpStatusCode.NotFound, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingLanguageResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task LocalizationMutations_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = "Unauthorized"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<AdminEventNewsCategoryDto> CreateCategoryAsync(string name)
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategories",
            new CreateEventNewsCategoryDto { Name = name });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AdminEventNewsCategoryDto>())!;
    }
}
