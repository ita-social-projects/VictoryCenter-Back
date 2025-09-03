using System.Net;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetById;

public class GetProgramById : BaseTestClass
{
    public GetProgramById(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetProgramById_ShouldReturnProgram(int programId)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/Programs/{programId}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        ProgramDto? responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);
        Assert.NotNull(responseContent);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task GetProgramById_ShouldReturnNotFound(int programId)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/Programs/{programId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
