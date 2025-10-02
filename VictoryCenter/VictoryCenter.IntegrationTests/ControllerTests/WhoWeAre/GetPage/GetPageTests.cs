using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Public.WhoWeArePage;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.WhoWeAre.GetPage;

public class GetPageTests : BaseTestClass
{
    public GetPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllSections_ShouldReturnAllSections()
    {
        var response = await Fixture.HttpClient.GetAsync($"/api/WhoWeArePage");
        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonSerializer.Deserialize<List<WhoWeArePageSectionDto>>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseContent?.Count > 0);
    }
}
