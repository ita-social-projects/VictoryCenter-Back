using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FaqQuestions.Search;

public class SearchFaqQuestionTests : BaseTestClass
{
    public SearchFaqQuestionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchFaqQuestion_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        string searchQuery = $"Faq question";

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/Faq/search?searchQuery={searchQuery}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchFaqQuestion_InvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        string searchQuery = $"";

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/Faq/search?searchQuery={searchQuery}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
