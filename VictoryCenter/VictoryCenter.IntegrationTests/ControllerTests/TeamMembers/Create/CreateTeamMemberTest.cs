using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Create;

[Collection("SharedIntegrationTests")]
public class CreateTeamMemberTest
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;

    public CreateTeamMemberTest(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task CreateTeamMember_ShouldReturnOk()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FirstName = "TestName",
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
            FirstName = "TestName",
            CategoryId = 10000,
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

    [Fact]
    public async Task CreateTeamMember_ShouldFail_InvalidFirstNameLength()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");
        var createTeamMemberDto = new CreateTeamMemberDto
        {
            FirstName = "A",
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
