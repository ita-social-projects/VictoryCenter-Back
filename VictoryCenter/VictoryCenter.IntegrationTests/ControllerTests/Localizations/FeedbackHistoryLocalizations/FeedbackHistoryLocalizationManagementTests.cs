using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.FeedbackHistoryLocalizations;

public class FeedbackHistoryLocalizationManagementTests : BaseTestClass
{
    private const string LocalizationsUrl = "/api/FeedbackHistoryLocalizations";
    private const string FeedbackHistoriesUrl = "/api/FeedbackHistories";

    public FeedbackHistoryLocalizationManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task LocalizationManagement_ShouldSupportAuthorizedCreateAndGet()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackHistory = await CreateFeedbackHistoryAsync();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = feedbackHistory.Id,
                LanguageId = language.Id,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var localization = await createResponse.Content.ReadFromJsonAsync<FeedbackHistoryLocalizationDto>();
        Assert.NotNull(localization);
        Assert.Equal(feedbackHistory.Id, localization.EntityId);
        Assert.Equal(language.Id, localization.Language.Id);
        Assert.Equal(language.Code, localization.Language.Code);
        Assert.Equal("English recovery story title", localization.Title);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);

        var getResponse = await Fixture.HttpClient.GetAsync($"{LocalizationsUrl}/entityId/{feedbackHistory.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var localizations = await getResponse.Content.ReadFromJsonAsync<List<FeedbackHistoryLocalizationDto>>();
        var single = Assert.Single(localizations!);
        Assert.Equal("English recovery story title", single.Title);
        Assert.Equal(
            "A detailed English translation of the recovery story content.",
            single.Story);
    }

    [Fact]
    public async Task CreateLocalization_ShouldRejectDuplicateEntityLanguagePair()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackHistory = await CreateFeedbackHistoryAsync();
        var payload = new CreateFeedbackHistoryLocalizationDto
        {
            EntityId = feedbackHistory.Id,
            LanguageId = language.Id,
            Title = "English recovery story title",
            Story = "A detailed English translation of the recovery story content."
        };

        var firstResponse = await Fixture.HttpClient.PostAsJsonAsync(LocalizationsUrl, payload);
        var duplicateResponse = await Fixture.HttpClient.PostAsJsonAsync(LocalizationsUrl, payload);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        Assert.Contains(
            FeedbackHistoryConstants.LocalizationAlreadyExists,
            await duplicateResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenTitleTooShort()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackHistory = await CreateFeedbackHistoryAsync();

        var response = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = feedbackHistory.Id,
                LanguageId = language.Id,
                Title = "Short",
                Story = "A detailed English translation of the recovery story content."
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnNotFoundForMissingResources()
    {
        const long missingId = long.MaxValue;
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackHistory = await CreateFeedbackHistoryAsync();

        var missingEntityResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = missingId,
                LanguageId = language.Id,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });
        var missingLanguageResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = feedbackHistory.Id,
                LanguageId = missingId,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });
        var getResponse = await Fixture.HttpClient.GetAsync($"{LocalizationsUrl}/entityId/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, missingEntityResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingLanguageResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<FeedbackHistoryDto> CreateFeedbackHistoryAsync()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            FeedbackHistoriesUrl,
            new CreateFeedbackHistoryDto
            {
                Title = "Original success story",
                Story = "Original Ukrainian story content for the feedback record.",
                Status = Status.Draft
            });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<FeedbackHistoryDto>())!;
    }
}
