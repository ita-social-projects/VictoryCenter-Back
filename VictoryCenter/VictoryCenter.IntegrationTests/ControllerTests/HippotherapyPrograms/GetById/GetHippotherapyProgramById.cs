using System.Net;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyPrograms.GetById;

public class GetHippotherapyProgramById : BaseTestClass
{
    public GetHippotherapyProgramById(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetProgramById_ShouldReturnProgram(int programId)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/HippotherapyPrograms/{programId}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        HippotherapyProgramDto? responseContent = JsonConvert.DeserializeObject<HippotherapyProgramDto>(responseString);
        Assert.NotNull(responseContent);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task GetProgramById_ShouldReturnNotFound(int programId)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/HippotherapyPrograms/{programId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
