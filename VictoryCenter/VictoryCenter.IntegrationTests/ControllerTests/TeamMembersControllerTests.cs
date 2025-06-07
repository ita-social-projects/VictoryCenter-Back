using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests;

public class TeamMembersControllerTests : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly VictoryCenterDbContext _dbContext;

    public TeamMembersControllerTests(VictoryCenterWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
    }

    [Fact]
    public async Task GetAllTestData_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/TeamMembers/GetTeamMembers");
        var responseString = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<List<TeamMemberDto>>(responseString, options);
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }

    [Fact]
    public async Task GetTestDataById_ShouldReturnOk()
    {
        var existingEntity = await _dbContext.TestEntities.FirstOrDefaultAsync();
        
        var response = await _client.GetAsync($"/api/TeamMembers/GetTeamMemberById/{existingEntity!.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<TestDataDto>(responseString, options);
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
    }

    [Fact]
    public async Task GetTestDataById_ShouldFail_NotFound()
    {
        var response = await _client.GetAsync($"/api/Test/GetTestData/{-1}");
        
        Assert.False(response.IsSuccessStatusCode);
    }
}
