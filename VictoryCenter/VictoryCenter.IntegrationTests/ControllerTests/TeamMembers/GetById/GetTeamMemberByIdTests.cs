using System.Net;
using System.Text.Json;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetById;

[Collection("SharedIntegrationTests")]
public class GetTeamMemberByIdTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly IntegrationTestDbFixture _fixture;

    public GetTeamMemberByIdTests(IntegrationTestDbFixture fixture, ITestOutputHelper output)
    {
        _output = output;
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();

        var count = await _fixture.DbContext.TeamMembers.CountAsync();
        _output.WriteLine($"TeamMembers count after seeding: {count}");

        if (count == 0)
        {
            throw new InvalidOperationException("No TeamMembers were seeded. Check your seeder implementation.");
        }
    }

    public async Task DisposeAsync()
    {
        await _fixture.SeederManager.DisposeAllAsync();
    }

    [Fact]
    public async Task GetTeamMemberById_ShouldReturnOk()
    {
        var all = await _fixture.DbContext.TeamMembers
            .Include(tm => tm.Category)
            .ToListAsync();

        _output.WriteLine($"Found {all.Count} team members:");
        foreach (var a in all)
        {
            _output.WriteLine($"ID: {a.Id}, Name: {a.FullName}, CategoryId: {a.CategoryId}");
        }

        // Arrange
        var existingEntity = await _fixture.DbContext.TeamMembers.Include(tm => tm.Category).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        // Act
        var response = await _fixture.HttpClient.GetAsync($"api/TeamMembers/{existingEntity!.Id}");
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, options);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Multiple(
            () => Assert.Equal(existingEntity.Id, responseContent.Id),
            () => Assert.Equal(existingEntity.FullName, responseContent.FullName),
            () => Assert.Equal(existingEntity.Category.Id, responseContent.CategoryId),
            () => Assert.Equal(existingEntity.Description, responseContent.Description),
            () => Assert.Equal(existingEntity.Email, responseContent.Email),
            () => Assert.Equal(existingEntity.Status, responseContent.Status),
            () => Assert.Equal(existingEntity.Priority, responseContent.Priority));
    }

    [Fact]
    public async Task GetTeamMemberById_ShouldFail_NotFound()
    {
        var response = await _fixture.HttpClient.GetAsync($"api/TeamMembers/{-1}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
