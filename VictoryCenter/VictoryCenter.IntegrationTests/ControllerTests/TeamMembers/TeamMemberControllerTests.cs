using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers;

public class TeamMemberControllerTests : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly VictoryCenterDbContext _dbContext;

    public TeamMemberControllerTests(VictoryCenterWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
    }

    [Fact]
    public async Task UpdateTestData_ShouldReturnOk()
    {
        await VictoryCenterDatabaseSeeder.SeedDataAsync(_dbContext);

        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Category 1");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FirstName = "TestName",
            LastName = "TestLastName",
            MiddleName = "TestMiddleName",
            CategoryId = category.Id,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
        };

        var serializedDto = JsonConvert.SerializeObject(createTeamMemberDto);

        var response = await _client.PostAsync("/api/TeamMembers/CreateTeamMember", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTestData_ShouldFail_InvalidCategoryId()
    {
        await VictoryCenterDatabaseSeeder.SeedDataAsync(_dbContext);

        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Category 1");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FirstName = "TestName",
            LastName = "TestLastName",
            MiddleName = "TestMiddleName",
            CategoryId = 10000,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
        };

        var serializedDto = JsonConvert.SerializeObject(createTeamMemberDto);

        var response = await _client.PostAsync("/api/TeamMembers/CreateTeamMember", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTestData_ShouldFail_InvalidFirstNameLength()
    {
        await VictoryCenterDatabaseSeeder.SeedDataAsync(_dbContext);

        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Category 1");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FirstName = "A",
            LastName = "TestLastName",
            MiddleName = "TestMiddleName",
            CategoryId = category.Id,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
        };

        var serializedDto = JsonConvert.SerializeObject(createTeamMemberDto);

        var response = await _client.PostAsync("/api/TeamMembers/CreateTeamMember", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        Assert.False(response.IsSuccessStatusCode);
    }
}
