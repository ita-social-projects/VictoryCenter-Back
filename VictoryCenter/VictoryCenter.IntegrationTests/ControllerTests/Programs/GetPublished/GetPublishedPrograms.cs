using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetPublished;

public class GetPublishedPrograms : BaseTestClass
{
    public GetPublishedPrograms(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedPrograms_ShouldReturnPublishedPrograms()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/Programs/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ProgramDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
