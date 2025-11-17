using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.SupportOptions.GetAll;

public class GetAllSupportOptionsTests : BaseTestClass
{
    public GetAllSupportOptionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SupportOptions_ShouldReturnAll()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/SupportOptions/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<SupportOptionsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<SupportOptionsDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
