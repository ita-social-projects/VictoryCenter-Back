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