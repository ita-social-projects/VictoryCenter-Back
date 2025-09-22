using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FaqQuestions.GetFiltered;

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
