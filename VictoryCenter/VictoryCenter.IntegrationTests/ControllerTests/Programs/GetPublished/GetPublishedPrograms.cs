using Newtonsoft.Json;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetPublished;

[Collection("SharedIntegrationTests")]
public class GetPublishedPrograms : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

    public GetPublishedPrograms(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException(
            "SeederManager is not registered in the service collection.");
    }

    public async Task InitializeAsync()
    {
        await _seederManager.DisposeAllAsync();
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPublishedPrograms_ShouldReturnPublishedPrograms()
    {
        var response = await _httpClient.GetAsync("/api/Programs/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
