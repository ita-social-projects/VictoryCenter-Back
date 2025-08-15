using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Public.FAQ;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.GetPublished;

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
        var responseString = await response.Content.ReadAsStringAsync();
        var faqDtos = JsonSerializer.Deserialize<List<PublishedFaqQuestionDto>>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(faqDtos);
        Assert.NotEmpty(faqDtos);
    }
}
