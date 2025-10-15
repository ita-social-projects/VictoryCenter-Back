using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.WhoWeAre.GetBySection;

public class GetBySectionTypeTests : BaseTestClass
{
    public GetBySectionTypeTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetSection_ShouldReturnSection()
    {
        var response = await Fixture.HttpClient.GetAsync($"/api/WhoWeAre/{(int)SectionType.Main}");
        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonSerializer.Deserialize<WhoWeAreSectionDto>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(SectionType.Main, responseContent?.SectionType);
        Assert.True(responseContent?.Contents.Count > 0);
    }
}
