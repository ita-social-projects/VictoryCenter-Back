using System.Net;
using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetTeamMembers;

public class GetTeamMembers : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly VictoryCenterDbContext _dbContext;

    public GetTeamMembers(VictoryCenterWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
    }

    [Fact]
    public async Task GetTeamMembers_ShouldReturnOk()
    {
        var response = await _client.GetAsync("api/TeamMembers/GetTeamMembers");
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<List<TeamMemberDto>>(responseString, options);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
    }
}
