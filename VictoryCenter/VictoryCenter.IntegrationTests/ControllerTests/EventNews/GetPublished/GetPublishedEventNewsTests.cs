using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.GetPublished;

public class GetPublishedEventNewsTests : BaseTestClass
{
    public GetPublishedEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnOnlyPublishedItems()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/EventNews/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedEventNewsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
        Assert.All(responseContent, item => Assert.NotNull(item.Localizations));
        Assert.All(responseContent, item => Assert.NotNull(item.Categories));
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenTake4_ShouldReturnAtMost4Items()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/EventNews/published?take=4");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedEventNewsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.True(responseContent.Count() <= 4);
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldOrderItemsByPriority()
    {
        // Arrange
        var publishedEventIds = Fixture.DbContext.EventNews
            .Where(eventNews => eventNews.Status == Status.Published)
            .Select(eventNews => eventNews.Id)
            .Take(3)
            .ToList();

        Assert.Equal(3, publishedEventIds.Count);

        var links = Fixture.DbContext.EventNewsEventNewsCategories
            .Where(link => publishedEventIds.Contains(link.EventsNewsId))
            .ToList();

        foreach (var link in links.Where(link => link.EventsNewsId == publishedEventIds[0]))
        {
            link.Priority = 2;
        }

        foreach (var link in links.Where(link => link.EventsNewsId == publishedEventIds[1]))
        {
            link.Priority = 0;
        }

        foreach (var link in links.Where(link => link.EventsNewsId == publishedEventIds[2]))
        {
            link.Priority = 1;
        }

        await Fixture.DbContext.SaveChangesAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("/api/EventNews/published/");

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent =
            JsonConvert.DeserializeObject<List<PublishedEventNewsDto>>(responseString);

        Assert.NotNull(responseContent);

        var testedIds = publishedEventIds.ToHashSet();

        var testedItems = responseContent
            .Where(item => testedIds.Contains(item.Id))
            .Select(item => item.Id)
            .ToList();

        Assert.Equal(
            [publishedEventIds[1], publishedEventIds[2], publishedEventIds[0]],
            testedItems);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(EventNewsConstants.PublishedTakeMaxValue + 1)]
    public async Task GetPublishedEventNews_WhenTakeIsOutOfRange_ShouldReturnBadRequest(int take)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/EventNews/published?take={take}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
