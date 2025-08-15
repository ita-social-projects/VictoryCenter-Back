using System.Net;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.GetFiltered;

public class GetFilteredQuestionsTests : BaseTestClass
{
    public GetFilteredQuestionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetFilteredQuestions_ShouldReturnFaqQuestions()
    {
        var response = await Fixture.HttpClient.GetAsync(new Uri("/api/faq", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
