using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetTeamMembers;

[Collection("SharedIntegrationTests")]
public class GetTeamMembersTests : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public GetTeamMembersTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task GetTeamMembers_ShouldReturnOk()
    {
        var response = await _httpClient.GetAsync("api/TeamMembers/");
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<List<TeamMemberDto>>(responseString, options);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
