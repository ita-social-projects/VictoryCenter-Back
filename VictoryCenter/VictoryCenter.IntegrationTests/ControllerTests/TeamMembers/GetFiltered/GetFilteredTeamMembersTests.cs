using System.Net;
using System.Text.Json;
<<<<<<< HEAD
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetFiltered;

[Collection("SharedIntegrationTests")]
public class GetFilteredTeamMembersTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

=======
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetFiltered;

public class GetFilteredTeamMembersTests : BaseTestClass
{
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    public GetFilteredTeamMembersTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
<<<<<<< HEAD
        _httpClient = fixture.HttpClient;
        _seederManager = fixture.SeederManager
            ?? throw new InvalidOperationException("SeederManager is not registered in the service collection.");
=======
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    }

    public async Task InitializeAsync()
    {
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

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
