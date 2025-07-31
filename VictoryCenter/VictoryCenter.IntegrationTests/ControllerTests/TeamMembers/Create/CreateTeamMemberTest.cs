using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Create;

[Collection("SharedIntegrationTests")]
public class CreateTeamMemberTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly SeederManager _seederManager;

    public CreateTeamMemberTest(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _seederManager = fixture.SeederManager
            ?? throw new InvalidOperationException("SeederManager is not registered in the service collection.");
    }

    public async Task InitializeAsync()
    {
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateTeamMember_ShouldReturnOk()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FullName = "TestName",
            CategoryId = category.Id,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
        };

        var serializedDto = JsonConvert.SerializeObject(createTeamMemberDto);

        var response = await _client.PostAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateTeamMember_ShouldFail_InvalidCategoryId()
    {
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FullName = "TestName",
            CategoryId = 10000,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
        };

        var serializedDto = JsonConvert.SerializeObject(createTeamMemberDto);

        var response = await _client.PostAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateTeamMember_ShouldFail_InvalidFullNameLength()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FullName = "A",
            CategoryId = category.Id,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
        };

        var serializedDto = JsonConvert.SerializeObject(createTeamMemberDto);

        var response = await _client.PostAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }
}
