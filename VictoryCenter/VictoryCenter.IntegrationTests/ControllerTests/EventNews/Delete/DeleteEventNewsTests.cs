using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.Delete;

public class DeleteEventNewsTests : BaseTestClass
{
    private const string EndpointUri = "/api/EventNews";

    public DeleteEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteEventNews_ShouldDeleteAggregateAndPreserveSharedEntities()
    {
        const long eventNewsId = 1;
        const long imageId = 1;
        const long languageId = 1;
        var eventNews = await Fixture.DbContext.EventNews
            .Include(entity => entity.Categories)
            .SingleAsync(entity => entity.Id == eventNewsId);
        var categoryIds = eventNews.Categories.Select(category => category.Id).ToList();

        eventNews.PreviewImageId = imageId;
        eventNews.Localizations.Add(new EventNewsLocalization
        {
            EntityId = eventNewsId,
            LanguageId = languageId,
            Title = "Community Support Event",
            Description = "Details about the community support event",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var response = await Fixture.HttpClient.DeleteAsync($"{EndpointUri}/{eventNewsId}");
        var deletedId = await response.Content.ReadFromJsonAsync<long>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(eventNewsId, deletedId);

        Fixture.DbContext.ChangeTracker.Clear();
        Assert.False(await Fixture.DbContext.EventNews
            .AsNoTracking()
            .AnyAsync(entity => entity.Id == eventNewsId));
        Assert.False(await Fixture.DbContext.EventNewsLocalizations
            .AsNoTracking()
            .AnyAsync(localization => localization.EntityId == eventNewsId));
        Assert.False(await Fixture.DbContext.EventNewsCategories
            .AsNoTracking()
            .AnyAsync(category => category.EventsNews.Any(entity => entity.Id == eventNewsId)));
        Assert.True(await Fixture.DbContext.Images
            .AsNoTracking()
            .AnyAsync(image => image.Id == imageId));
        Assert.True(await Fixture.DbContext.LocalizationLanguages
            .AsNoTracking()
            .AnyAsync(language => language.Id == languageId));
        Assert.Equal(
            categoryIds.Count,
            await Fixture.DbContext.EventNewsCategories
                .AsNoTracking()
                .CountAsync(category => categoryIds.Contains(category.Id)));
    }

    [Fact]
    public async Task DeleteEventNews_ShouldDeletePublishedEventNews()
    {
        const long publishedEventNewsId = 2;

        var response = await Fixture.HttpClient.DeleteAsync($"{EndpointUri}/{publishedEventNewsId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(publishedEventNewsId, await response.Content.ReadFromJsonAsync<long>());
        Fixture.DbContext.ChangeTracker.Clear();
        Assert.False(await Fixture.DbContext.EventNews
            .AsNoTracking()
            .AnyAsync(entity => entity.Id == publishedEventNewsId));
    }

    [Fact]
    public async Task DeleteEventNews_ShouldReturnNotFound_WhenEventNewsDoesNotExist()
    {
        const long missingId = long.MaxValue;

        var response = await Fixture.HttpClient.DeleteAsync($"{EndpointUri}/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEventNews_ShouldReturnNotFound_WhenRequestIsRepeated()
    {
        const long eventNewsId = 1;

        var firstResponse = await Fixture.HttpClient.DeleteAsync($"{EndpointUri}/{eventNewsId}");
        var secondResponse = await Fixture.HttpClient.DeleteAsync($"{EndpointUri}/{eventNewsId}");

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, secondResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteEventNews_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.DeleteAsync($"{EndpointUri}/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.True(await Fixture.DbContext.EventNews
            .AsNoTracking()
            .AnyAsync(entity => entity.Id == 1));
    }
}
