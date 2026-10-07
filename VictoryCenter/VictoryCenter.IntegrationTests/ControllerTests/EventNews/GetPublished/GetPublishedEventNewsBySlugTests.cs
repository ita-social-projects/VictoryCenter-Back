using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.GetPublished;

public class GetPublishedEventNewsBySlugTests : BaseTestClass
{
    private const string BaseUrl = "/api/EventNews/published";

    public GetPublishedEventNewsBySlugTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedEventNewsBySlug_WhenPublished_ShouldReturnOkWithDetails()
    {
        var eventNews = await Fixture.DbContext.EventNews
            .AsNoTracking()
            .FirstAsync(item => item.Status == Status.Published && item.Slug != null);

        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/{eventNews.Slug}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PublishedEventNewsDetailsDto>();
        Assert.NotNull(dto);
        Assert.Equal(eventNews.Id, dto.Id);
        Assert.Equal(eventNews.Slug, dto.Slug);
        Assert.NotNull(dto.Categories);
        Assert.NotNull(dto.Localizations);
    }

    [Fact]
    public async Task GetPublishedEventNewsBySlug_WhenSlugDoesNotExist_ShouldReturnNotFound()
    {
        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/slug-that-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPublishedEventNewsBySlug_WhenNewsIsNotPublished_ShouldNotReturnIt()
    {
        var eventNews = await Fixture.DbContext.EventNews
            .FirstAsync(item => item.Status == Status.Published && item.Slug != null);
        var slug = eventNews.Slug;
        eventNews.Status = Status.Draft;
        await Fixture.DbContext.SaveChangesAsync();

        try
        {
            var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/{slug}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            eventNews.Status = Status.Published;
            await Fixture.DbContext.SaveChangesAsync();
        }
    }
}
