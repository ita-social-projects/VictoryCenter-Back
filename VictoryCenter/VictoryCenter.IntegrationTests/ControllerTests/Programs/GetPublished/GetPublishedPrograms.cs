using Newtonsoft.Json;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetPublished;

[Collection("SharedIntegrationTests")]
public class GetPublishedPrograms : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public GetPublishedPrograms(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPublishedPrograms_ShouldReturnPublishedPrograms()
    {
        var response = await _fixture.HttpClient.GetAsync("/api/Programs/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
