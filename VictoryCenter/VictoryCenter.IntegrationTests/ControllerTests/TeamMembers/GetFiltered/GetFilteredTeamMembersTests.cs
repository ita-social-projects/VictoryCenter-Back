using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetFiltered;

public class GetFilteredTeamMembersTests : BaseTestClass
{
    public GetFilteredTeamMembersTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetTeamMembers_ShouldReturnOk()
    {
        var response = await Fixture.HttpClient.GetAsync("api/TeamMembers/");
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
