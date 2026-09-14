using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.Update;

public class UpdateFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public UpdateFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateFeedbackHistory_ValidRequest_ShouldUpdateEntity()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Updated Valid Title Here",
            Story = "Updated story content that satisfies validation.",
            ImageId = null,
            Status = Status.Published
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{existingEntity.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        FeedbackHistoryDto? responseContent = JsonSerializer.Deserialize<FeedbackHistoryDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateDto.Title, responseContent.Title);
        Assert.Equal(updateDto.Story, responseContent.Story);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("TooShort")]
    public async Task UpdateFeedbackHistory_InvalidTitle_ShouldReturnBadRequest(string? invalidTitle)
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = invalidTitle!,
            Story = "Valid story content for testing validation.",
            ImageId = null,
            Status = Status.Published
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{existingEntity.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateFeedbackHistory_NotFound_ShouldReturnNotFound(long testId)
    {
        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Non Existing Entity Title",
            Story = "Story content for not found scenario.",
            ImageId = null,
            Status = Status.Published
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{testId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedbackHistory_NonExistentImageId_ShouldReturnNotFound()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Updated Valid Title Here",
            Story = "Updated story content that satisfies validation.",
            ImageId = long.MaxValue,
            Status = Status.Published
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{existingEntity.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedbackHistory_TitleOrStoryChanged_ShouldMarkExistingTranslationOutdated()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");

        var createLocalizationResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/FeedbackHistoryLocalizations",
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = existingEntity.Id,
                LanguageId = language.Id,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });
        createLocalizationResponse.EnsureSuccessStatusCode();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Updated Valid Title Here",
            Story = "Updated story content that satisfies validation.",
            ImageId = null,
            Status = Status.Published
        };

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync($"{BaseUrl}/{existingEntity.Id}", updateDto);
        updateResponse.EnsureSuccessStatusCode();

        var responseDto = await updateResponse.Content.ReadFromJsonAsync<FeedbackHistoryDto>();
        Assert.NotNull(responseDto);
        var localization = Assert.Single(responseDto.Localizations);
        Assert.Equal(TranslationStatus.Outdated, localization.TranslationStatus);
    }

    [Fact]
    public async Task UpdateFeedbackHistory_OnlyImageOrStatusChanged_ShouldNotMarkExistingTranslationOutdated()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync(l => l.Code == "en");

        var createLocalizationResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/FeedbackHistoryLocalizations",
            new CreateFeedbackHistoryLocalizationDto
            {
                EntityId = existingEntity.Id,
                LanguageId = language.Id,
                Title = "English recovery story title",
                Story = "A detailed English translation of the recovery story content."
            });
        createLocalizationResponse.EnsureSuccessStatusCode();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = existingEntity.Title,
            Story = existingEntity.Story,
            ImageId = null,
            Status = Status.Published
        };

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync($"{BaseUrl}/{existingEntity.Id}", updateDto);
        updateResponse.EnsureSuccessStatusCode();

        var responseDto = await updateResponse.Content.ReadFromJsonAsync<FeedbackHistoryDto>();
        Assert.NotNull(responseDto);
        var localization = Assert.Single(responseDto.Localizations);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
    }

    private async Task<FeedbackHistory> CreateTestFeedbackHistoryAsync()
    {
        var entity = new FeedbackHistory
        {
            Title = "Original Valid Title",
            Story = "Original story text that meets the length requirements.",
            ImageId = null,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Draft
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}