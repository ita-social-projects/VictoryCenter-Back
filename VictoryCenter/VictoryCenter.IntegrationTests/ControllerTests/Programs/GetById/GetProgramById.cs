using System.Net;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetById;

[Collection("SharedIntegrationTests")]
public class GetProgramById : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public GetProgramById(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetProgramById_ShouldReturnProgram(int programId)
    {
        var response = await _fixture.HttpClient.GetAsync($"/api/Program/{programId}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);
        Assert.NotNull(responseContent);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task GetProgramById_ShouldReturnNotFound(int programId)
    {
        var response = await _fixture.HttpClient.GetAsync($"/api/Program/{programId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
