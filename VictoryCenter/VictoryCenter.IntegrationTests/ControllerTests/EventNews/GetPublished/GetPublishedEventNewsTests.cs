using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Public.EventNews;
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
    public async Task GetPublishedEventNews_ShouldReturnItemsSortedByPublishedAtDescending()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/EventNews/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        List<PublishedEventNewsDto>? responseContent =
            JsonConvert.DeserializeObject<List<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);

        var publishedDates = responseContent
            .Select(e => e.PublishedAt ?? DateTimeOffset.MinValue)
            .ToList();
        Assert.Equal(publishedDates.OrderByDescending(d => d).ToList(), publishedDates);
    }
}
