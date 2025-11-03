using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Public.Donate.SupportOptions;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.SupportOptions.GetPublished;

public class GetPublishedSupportOptionsTests : BaseTestClass
{
    public GetPublishedSupportOptionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SupportOptions_ShouldReturnAll()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/SupportOptions/published/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedSupportOptionsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<PublishedSupportOptionsDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
