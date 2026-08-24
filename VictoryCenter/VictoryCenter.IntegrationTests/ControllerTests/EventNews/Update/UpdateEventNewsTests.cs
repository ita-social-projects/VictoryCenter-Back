using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.Update;

public class UpdateEventNewsTests : BaseTestClass
{
    private const string EndpointUri = "/api/EventNews";

    public UpdateEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateEventNews_ShouldSynchronizeAggregateAndBeIdempotent()
    {
        var originalCreatedAt = await Fixture.DbContext.EventNews
            .Where(eventNews => eventNews.Id == 1)
            .Select(eventNews => eventNews.CreatedAt)
            .SingleAsync();

        var firstDto = PublishedDto(
            categoryIds: [4, 5],
            localizations:
            [
                Localization(1, "Updated Integration Event", "Updated integration description"),
                Localization(2, "Updated Community Workshop", "Updated community workshop description"),
            ]);

        var firstResponse = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/1", firstDto);
        var firstContent = await firstResponse.Content.ReadFromJsonAsync<EventNewsDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.NotNull(firstContent);
        Assert.Equal("updated-integration-event", firstContent.Slug);
        Assert.Equal(Status.Published, firstContent.Status);
        Assert.Equal([4, 5], firstContent.Categories.Select(category => category.Id).OrderBy(id => id));
        Assert.Equal([1, 2], firstContent.Localizations.Select(item => item.Language.Id).OrderBy(id => id));
        Assert.NotNull(firstContent.PreviewImage);
        Assert.NotNull(firstContent.BackgroundImage);

        var secondDto = PublishedDto(
            categoryIds: [5],
            localizations:
            [
                Localization(2, "Community Charity Day", "Final details for the community charity day"),
            ]);

        var secondResponse = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/1", secondDto);
        var repeatResponse = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/1", secondDto);
        var secondContent = await secondResponse.Content.ReadFromJsonAsync<EventNewsDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, repeatResponse.StatusCode);
        Assert.NotNull(secondContent);
        Assert.Equal("community-charity-day", secondContent.Slug);
        Assert.Equal(5, Assert.Single(secondContent.Categories).Id);
        Assert.Equal(2, Assert.Single(secondContent.Localizations).Language.Id);

        Fixture.DbContext.ChangeTracker.Clear();
        var persisted = await Fixture.DbContext.EventNews
            .Include(eventNews => eventNews.Categories)
            .Include(eventNews => eventNews.Localizations)
            .SingleAsync(eventNews => eventNews.Id == 1);

        Assert.Equal(originalCreatedAt, persisted.CreatedAt);
        Assert.Equal(5, Assert.Single(persisted.Categories).Id);
        var persistedLocalization = Assert.Single(persisted.Localizations);
        Assert.Equal(2, persistedLocalization.LanguageId);
        Assert.Equal(TranslationStatus.Relevant, persistedLocalization.TranslationStatus);
    }

    [Fact]
    public async Task UpdateEventNews_ToEmptyDraft_ShouldRemoveContentAndHidePublicItem()
    {
        var dto = new UpdateEventNewsDto
        {
            Status = Status.Draft,
            Localizations = [new CreateEventNewsLocalizationDto { LanguageId = 1 }]
        };

        var response = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/2", dto);
        var content = await response.Content.ReadFromJsonAsync<EventNewsDto>(JsonOptions);
        var publicItems = await Fixture.HttpClient.GetFromJsonAsync<List<PublishedEventNewsDto>>(
            "/api/EventNews/published",
            JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(Status.Draft, content.Status);
        Assert.Null(content.Slug);
        Assert.Empty(content.Categories);
        Assert.Empty(content.Localizations);
        Assert.NotNull(publicItems);
        Assert.DoesNotContain(publicItems, eventNews => eventNews.Id == 2);
    }

    [Fact]
    public async Task UpdateEventNews_WhenEntityDoesNotExist_ShouldReturnNotFound()
    {
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"{EndpointUri}/{long.MaxValue}",
            new UpdateEventNewsDto { Status = Status.Draft });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventNews_WhenIdIsInvalid_ShouldReturnBadRequest()
    {
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"{EndpointUri}/0",
            new UpdateEventNewsDto { Status = Status.Draft });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePublishedEventNews_WhenRequiredFieldsAreMissing_ShouldReturnBadRequest()
    {
        var response = await Fixture.HttpClient.PutAsJsonAsync(
            $"{EndpointUri}/1",
            new UpdateEventNewsDto { Status = Status.Published });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventNews_WhenCategoryDoesNotExist_ShouldReturnNotFound()
    {
        var dto = PublishedDto(categoryIds: [long.MaxValue]);

        var response = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/1", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventNews_WhenImageDoesNotExist_ShouldReturnNotFound()
    {
        var dto = PublishedDto() with { PreviewImageId = long.MaxValue };

        var response = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/1", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventNews_WhenLanguageDoesNotExist_ShouldReturnNotFound()
    {
        var dto = PublishedDto(localizations:
        [
            Localization(long.MaxValue, "Missing Language Event", "Missing language description"),
        ]);

        var response = await Fixture.HttpClient.PutAsJsonAsync($"{EndpointUri}/1", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventNews_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.PutAsJsonAsync(
            $"{EndpointUri}/1",
            new UpdateEventNewsDto { Status = Status.Draft });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static UpdateEventNewsDto PublishedDto(
        List<long>? categoryIds = null,
        List<CreateEventNewsLocalizationDto>? localizations = null)
    {
        return new UpdateEventNewsDto
        {
            Resource = "https://example.com/integration-update",
            PublishedAt = DateTimeOffset.UtcNow,
            Status = Status.Published,
            PreviewImageId = 1,
            BackgroundImageId = 2,
            CategoryIds = categoryIds ?? [1],
            Localizations = localizations ??
            [
                Localization(1, "Default Integration Event", "Default integration description"),
            ]
        };
    }

    private static CreateEventNewsLocalizationDto Localization(
        long languageId,
        string title,
        string description)
    {
        return new CreateEventNewsLocalizationDto
        {
            LanguageId = languageId,
            Title = title,
            Description = description
        };
    }
}
