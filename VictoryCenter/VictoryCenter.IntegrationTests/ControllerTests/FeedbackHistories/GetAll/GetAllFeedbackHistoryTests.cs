using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.GetAll;

public class GetAllFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public GetAllFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllFeedbackHistories_ShouldReturnOkAndListOfEntities()
    {
        await CreateTestFeedbackHistoryAsync();

        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<FeedbackHistoryDto>>(responseString, JsonOptions);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
        Assert.Contains(responseContent, item => item.Title == "Title For GetAll Test");
    }

    [Fact]
    public async Task GetAllFeedbackHistories_ShouldIncludeTranslationForRecordWithLocalization()
    {
        var entity = await CreateTestFeedbackHistoryAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");

        var createLocalizationResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/FeedbackHistoryLocalizations",
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = entity.Id,
                LanguageId = language.Id,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });
        createLocalizationResponse.EnsureSuccessStatusCode();

        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/");
        var responseContent = await response.Content.ReadFromJsonAsync<List<FeedbackHistoryDto>>();

        var item = Assert.Single(responseContent!, x => x.Id == entity.Id);
        var localization = Assert.Single(item.Localizations);
        Assert.Equal(language.Id, localization.Language.Id);
        Assert.Equal("en", localization.Language.Code);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
    }

    [Fact]
    public async Task GetAllFeedbackHistories_ShouldReturnEmptyLocalizations_WhenRecordHasNoTranslation()
    {
        var entity = await CreateTestFeedbackHistoryAsync();

        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/");
        var responseContent = await response.Content.ReadFromJsonAsync<List<FeedbackHistoryDto>>();

        var item = Assert.Single(responseContent!, x => x.Id == entity.Id);
        Assert.Empty(item.Localizations);
    }

    [Fact]
    public async Task GetAllFeedbackHistories_ShouldFilterByTranslationStatus()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var missingEntity = await CreateTestFeedbackHistoryAsync();
        var relevantEntity = await CreateTestFeedbackHistoryAsync();
        var outdatedEntity = await CreateTestFeedbackHistoryAsync();

        await CreateLocalizationAsync(relevantEntity.Id, language.Id);
        await CreateLocalizationAsync(outdatedEntity.Id, language.Id);

        var outdateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{BaseUrl}/{outdatedEntity.Id}",
            new UpdateFeedbackHistoryDto
            {
                Title = "Changed title to outdate the translation",
                Story = outdatedEntity.Story,
                ImageId = null,
                Status = Status.Draft
            });
        outdateResponse.EnsureSuccessStatusCode();

        var missingItems = await GetFilteredAsync("Missing");
        Assert.Contains(missingItems, x => x.Id == missingEntity.Id);
        Assert.DoesNotContain(missingItems, x => x.Id == relevantEntity.Id);
        Assert.DoesNotContain(missingItems, x => x.Id == outdatedEntity.Id);

        var outdatedItems = await GetFilteredAsync("Outdated");
        Assert.Contains(outdatedItems, x => x.Id == outdatedEntity.Id);
        Assert.DoesNotContain(outdatedItems, x => x.Id == relevantEntity.Id);
        Assert.DoesNotContain(outdatedItems, x => x.Id == missingEntity.Id);

        var allItems = await GetFilteredAsync("All");
        Assert.Contains(allItems, x => x.Id == missingEntity.Id);
        Assert.Contains(allItems, x => x.Id == relevantEntity.Id);
        Assert.Contains(allItems, x => x.Id == outdatedEntity.Id);
    }

    private async Task<List<FeedbackHistoryDto>> GetFilteredAsync(string translationStatusFilter)
    {
        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/?TranslationStatusFilter={translationStatusFilter}");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<List<FeedbackHistoryDto>>())!;
    }

    private async Task CreateLocalizationAsync(long entityId, long languageId)
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/FeedbackHistoryLocalizations",
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = entityId,
                LanguageId = languageId,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });
        response.EnsureSuccessStatusCode();
    }

    private async Task<FeedbackHistory> CreateTestFeedbackHistoryAsync()
    {
        var entity = new FeedbackHistory
        {
            Title = "Title For GetAll Test",
            Story = "Story content for get all test that meets length requirements.",
            ImageId = null,
            CreatedAt = DateTimeOffset.UtcNow,
            Priority = 1,
            Status = Status.Draft
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}