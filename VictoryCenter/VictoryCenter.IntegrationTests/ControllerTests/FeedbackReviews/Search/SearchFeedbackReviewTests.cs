using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackReviews.Search;

public class SearchFeedbackReviewTests : BaseTestClass
{
    public SearchFeedbackReviewTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchFeedbackReviews_ValidRequest_ShouldReturnOk()
    {
        var response = await Fixture.HttpClient.GetAsync("api/FeedbackReviews/search?searchQuery=Ana");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchFeedbackReviews_InvalidRequest_ShouldReturnBadRequest()
    {
        var response = await Fixture.HttpClient.GetAsync("api/FeedbackReviews/search?searchQuery=ab");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}