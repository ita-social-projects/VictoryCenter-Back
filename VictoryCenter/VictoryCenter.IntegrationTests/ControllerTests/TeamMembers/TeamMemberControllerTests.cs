using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers;

[Collection("SharedIntegrationTests")]
public class TeamMemberControllerTests
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;

    public TeamMemberControllerTests(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task UpdateTestData_ShouldReturnOk()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "CreateMember");
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

        var response = await _client.PostAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTestData_ShouldFail_InvalidCategoryId()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "CreateMember");
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

        var response = await _client.PostAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTestData_ShouldFail_InvalidFirstNameLength()
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "CreateMember");
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

        var response = await _client.PostAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        Assert.False(response.IsSuccessStatusCode);
    }
}
