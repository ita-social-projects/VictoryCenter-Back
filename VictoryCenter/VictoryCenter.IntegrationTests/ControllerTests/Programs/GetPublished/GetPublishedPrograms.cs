using Newtonsoft.Json;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
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
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPublishedPrograms_ShouldReturnPublishedPrograms()
    {
        HttpResponseMessage response = await _fixture.HttpClient.GetAsync("/api/Programs/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ProgramDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
