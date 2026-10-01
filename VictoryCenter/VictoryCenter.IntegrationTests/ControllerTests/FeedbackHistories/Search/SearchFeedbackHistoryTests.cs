using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.Search;

public class SearchFeedbackHistoryTests : BaseTestClass
{
    public SearchFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchFeedbackHistories_ValidRequest_ShouldReturnOk()
    {
        var response = await Fixture.HttpClient.GetAsync("api/FeedbackHistories/search?searchQuery=Title");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchFeedbackHistories_InvalidRequest_ShouldReturnBadRequest()
    {
        var response = await Fixture.HttpClient.GetAsync("api/FeedbackHistories/search?searchQuery=ab");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}