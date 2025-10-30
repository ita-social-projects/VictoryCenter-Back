using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyPrograms.GetPublished;

public class GetHippotherapyPublishedPrograms : BaseTestClass
{
    public GetHippotherapyPublishedPrograms(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetHippotherapyPublishedPrograms_ShouldReturnPublishedPrograms()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/HippotherapyPrograms/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedHippotherapyProgramDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<PublishedHippotherapyProgramDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
