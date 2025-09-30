using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyPrograms.GetPublished;

public class GetHypotherapyPublishedPrograms : BaseTestClass
{
    public GetHypotherapyPublishedPrograms(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedPrograms_ShouldReturnPublishedPrograms()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/HypotherapyPrograms/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<HypotherapyProgramDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<HypotherapyProgramDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
