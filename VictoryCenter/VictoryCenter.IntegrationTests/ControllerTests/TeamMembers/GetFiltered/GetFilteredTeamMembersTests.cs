using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetFiltered;

[Collection("SharedIntegrationTests")]
public class GetFilteredTeamMembersTests
{
    private readonly HttpClient _httpClient;

    public GetFilteredTeamMembersTests(IntegrationTestDbFixture fixture)
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

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
