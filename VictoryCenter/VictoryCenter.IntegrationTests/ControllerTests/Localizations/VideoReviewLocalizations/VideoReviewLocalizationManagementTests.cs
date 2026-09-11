using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.VideoReviewLocalizations;

public class VideoReviewLocalizationManagementTests : BaseTestClass
{
    private const string LocalizationsUrl = "/api/VideoReviewLocalizations";
    private const string VideoReviewsUrl = "/api/VideoReviews";

    public VideoReviewLocalizationManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task LocalizationManagement_ShouldSupportAuthorizedCreateAndGet()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var videoReview = await CreateVideoReviewAsync();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateVideoReviewLocalizationDto
            {
                EntityId = videoReview.Id,
                LanguageId = language.Id,
                Title = "English video review title"
            });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var localization = await createResponse.Content.ReadFromJsonAsync<VideoReviewLocalizationDto>();
        Assert.NotNull(localization);
        Assert.Equal(videoReview.Id, localization.EntityId);
        Assert.Equal(language.Id, localization.Language.Id);
        Assert.Equal("English video review title", localization.Title);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);

        var getResponse = await Fixture.HttpClient.GetAsync($"{LocalizationsUrl}/entityId/{videoReview.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var localizations = await getResponse.Content.ReadFromJsonAsync<List<VideoReviewLocalizationDto>>();
        var single = Assert.Single(localizations!);
        Assert.Equal("English video review title", single.Title);
    }

    [Fact]
    public async Task UpdateLocalization_ShouldUpdateTitleAndMarkTranslationRelevant()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var videoReview = await CreateVideoReviewAsync();
        await CreateLocalizationAsync(videoReview.Id, language.Id);

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{LocalizationsUrl}/{videoReview.Id}/{language.Id}",
            new UpdateVideoReviewLocalizationDto { Title = "Updated English video review title" });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<VideoReviewLocalizationDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated English video review title", updated.Title);
        Assert.Equal(TranslationStatus.Relevant, updated.TranslationStatus);
    }

    [Fact]
    public async Task DeleteLocalization_ShouldDeleteLocalization()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var videoReview = await CreateVideoReviewAsync();
        await CreateLocalizationAsync(videoReview.Id, language.Id);

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"{LocalizationsUrl}/{videoReview.Id}/{language.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await Fixture.HttpClient.GetAsync($"{LocalizationsUrl}/entityId/{videoReview.Id}");
        var localizations = await getResponse.Content.ReadFromJsonAsync<List<VideoReviewLocalizationDto>>();
        Assert.Empty(localizations!);
    }

    [Fact]
    public async Task UpdateAndDeleteLocalization_ShouldReturnNotFound_WhenLocalizationDoesNotExist()
    {
        const long missingId = long.MaxValue;

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{LocalizationsUrl}/{missingId}/{missingId}",
            new UpdateVideoReviewLocalizationDto { Title = "Updated English video review title" });
        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"{LocalizationsUrl}/{missingId}/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateAndDeleteLocalization_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var updateResponse = await anonymousClient.PutAsJsonAsync(
            $"{LocalizationsUrl}/1/1",
            new UpdateVideoReviewLocalizationDto { Title = "Title" });
        var deleteResponse = await anonymousClient.DeleteAsync($"{LocalizationsUrl}/1/1");

        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task CreateLocalization_ShouldRejectDuplicateEntityLanguagePair()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var videoReview = await CreateVideoReviewAsync();
        var payload = new CreateVideoReviewLocalizationDto
        {
            EntityId = videoReview.Id,
            LanguageId = language.Id,
            Title = "English video review title"
        };

        var firstResponse = await Fixture.HttpClient.PostAsJsonAsync(LocalizationsUrl, payload);
        var duplicateResponse = await Fixture.HttpClient.PostAsJsonAsync(LocalizationsUrl, payload);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        Assert.Contains(
            VideoReviewConstants.LocalizationAlreadyExists,
            await duplicateResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CreateLocalization_ShouldFail_WhenTitleTooShort()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var videoReview = await CreateVideoReviewAsync();

        var response = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateVideoReviewLocalizationDto
            {
                EntityId = videoReview.Id,
                LanguageId = language.Id,
                Title = "Hi"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnNotFoundForMissingResources()
    {
        const long missingId = long.MaxValue;
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");
        var videoReview = await CreateVideoReviewAsync();

        var missingEntityResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateVideoReviewLocalizationDto
            {
                EntityId = missingId,
                LanguageId = language.Id,
                Title = "English video review title"
            });
        var missingLanguageResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateVideoReviewLocalizationDto
            {
                EntityId = videoReview.Id,
                LanguageId = missingId,
                Title = "English video review title"
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
            new CreateVideoReviewLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Title = "English video review title"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task CreateLocalizationAsync(long entityId, long languageId)
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            LocalizationsUrl,
            new CreateVideoReviewLocalizationDto
            {
                EntityId = entityId,
                LanguageId = languageId,
                Title = "English video review title"
            });
        response.EnsureSuccessStatusCode();
    }

    private async Task<VideoReviewDto> CreateVideoReviewAsync()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            VideoReviewsUrl,
            new CreateVideoReviewDto
            {
                Title = "Оригінальний відеовідгук",
                Link = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                Status = Status.Draft
            });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<VideoReviewDto>())!;
    }
}
