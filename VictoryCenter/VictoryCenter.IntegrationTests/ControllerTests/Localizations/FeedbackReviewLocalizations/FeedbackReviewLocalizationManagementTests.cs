using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.FeedbackReviewLocalizations;

public class FeedbackReviewLocalizationManagementTests : BaseTestClass
{
    private const string LocalizationsUrl = "/api/FeedbackReviewLocalizations";
    private const string FeedbackReviewsUrl = "/api/FeedbackReviews";

    public FeedbackReviewLocalizationManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task LocalizationManagement_ShouldSupportAuthorizedCreateAndGet()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackReview = await CreateFeedbackReviewAsync();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackReviewLocalizationDto
            {
                EntityId = feedbackReview.Id,
                LanguageId = language.Id,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review content."
            });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var localization = await createResponse.Content.ReadFromJsonAsync<FeedbackReviewLocalizationDto>();
        Assert.NotNull(localization);
        Assert.Equal(feedbackReview.Id, localization.EntityId);
        Assert.Equal(language.Id, localization.Language.Id);
        Assert.Equal("John Doe", localization.AuthorName);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);

        var getResponse = await Fixture.HttpClient.GetAsync($"{LocalizationsUrl}/entityId/{feedbackReview.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var localizations = await getResponse.Content.ReadFromJsonAsync<List<FeedbackReviewLocalizationDto>>();
        var single = Assert.Single(localizations!);
        Assert.Equal("John Doe", single.AuthorName);
        Assert.Equal("An English translation of the participant review content.", single.Text);
    }

    [Fact]
    public async Task CreateLocalization_ShouldRejectDuplicateEntityLanguagePair()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackReview = await CreateFeedbackReviewAsync();
        var payload = new CreateFeedbackReviewLocalizationDto
        {
            EntityId = feedbackReview.Id,
            LanguageId = language.Id,
            AuthorName = "John Doe",
            Text = "An English translation of the participant review content."
        };

        var firstResponse = await Fixture.HttpClient.PostAsJsonAsync(LocalizationsUrl, payload);
        var duplicateResponse = await Fixture.HttpClient.PostAsJsonAsync(LocalizationsUrl, payload);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        Assert.Contains(
            FeedbackReviewConstants.LocalizationAlreadyExists,
            await duplicateResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenAuthorNameTooShort()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackReview = await CreateFeedbackReviewAsync();

        var response = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackReviewLocalizationDto
            {
                EntityId = feedbackReview.Id,
                LanguageId = language.Id,
                AuthorName = "Jo",
                Text = "An English translation of the participant review content."
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnNotFoundForMissingResources()
    {
        const long missingId = long.MaxValue;
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var feedbackReview = await CreateFeedbackReviewAsync();

        var missingEntityResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackReviewLocalizationDto
            {
                EntityId = missingId,
                LanguageId = language.Id,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review content."
            });
        var missingLanguageResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateFeedbackReviewLocalizationDto
            {
                EntityId = feedbackReview.Id,
                LanguageId = missingId,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review content."
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
            new CreateFeedbackReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                AuthorName = "John Doe",
                Text = "An English translation of the participant review content."
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<FeedbackReviewDto> CreateFeedbackReviewAsync()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            FeedbackReviewsUrl,
            new CreateFeedbackReviewDto
            {
                AuthorName = "Оригінальний автор",
                Text = "Оригінальний текст відгуку учасника програми.",
                Status = Status.Draft
            });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<FeedbackReviewDto>())!;
    }
}
