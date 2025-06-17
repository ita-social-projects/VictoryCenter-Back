using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetTeamMemberById;

public class GetTeamMemberById : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly VictoryCenterDbContext _dbContext;

    public GetTeamMemberById(VictoryCenterWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
    }

    [Fact]
    public async Task GetTestDataById_ShouldReturnOk()
    {
        var existingEntity = await _dbContext.TeamMembers.FirstOrDefaultAsync();

        var response = await _client.GetAsync($"api/TeamMembers/{existingEntity!.Id}");
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, options);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
    }

    [Fact]
    public async Task GetTestDataById_ShouldFail_NotFound()
    {
        var response = await _client.GetAsync($"api/TeamMembers/{-1}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
