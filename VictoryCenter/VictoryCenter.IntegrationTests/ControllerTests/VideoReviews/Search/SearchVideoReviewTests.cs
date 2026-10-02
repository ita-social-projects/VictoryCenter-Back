using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.VideoReviews.Search;

public class SearchVideoReviewTests : BaseTestClass
{
    public SearchVideoReviewTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchVideoReviews_ValidRequest_ShouldReturnOk()
    {
        var response = await Fixture.HttpClient.GetAsync("api/VideoReviews/search?searchQuery=Title");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchVideoReviews_InvalidRequest_ShouldReturnBadRequest()
    {
        var response = await Fixture.HttpClient.GetAsync("api/VideoReviews/search?searchQuery=ab");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
