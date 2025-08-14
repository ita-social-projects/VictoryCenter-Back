using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetFiltered;

[Collection("SharedIntegrationTests")]
public class GetFilteredTeamMembersTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public GetFilteredTeamMembersTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetTeamMembers_ShouldReturnOk()
    {
        var response = await _fixture.HttpClient.GetAsync("api/TeamMembers/");
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<PaginationResult<TeamMemberDto>>(responseString, options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent.Items);
        Assert.True(responseContent.TotalItemsCount > 0);
    }
}
