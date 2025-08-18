using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FaqQuestions.GetPublished;

public class GetPublishedFaqQuestionTests : BaseTestClass
{
    public GetPublishedFaqQuestionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedFaqQuestions_ShouldReturnSuccess()
    {
        var page = Fixture.DbContext.VisitorPages.First();

        var response = await Fixture.HttpClient.GetAsync(new Uri($"/api/faq/published/{page.Slug}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
