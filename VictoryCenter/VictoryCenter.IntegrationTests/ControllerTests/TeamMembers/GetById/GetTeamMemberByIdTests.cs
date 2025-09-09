using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetById;

[Collection("SharedIntegrationTests")]
public class GetTeamMemberByIdTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly SeederManager _seederManager;

=======
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetById;

public class GetTeamMemberByIdTests : BaseTestClass
{
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    public GetTeamMemberByIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
<<<<<<< HEAD
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException("SeederManager is not registered in the service collection.");
=======
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    }

    public async Task InitializeAsync()
    {
        await _seederManager.SeedAllAsync();
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetTeamMemberById_ShouldReturnOk()
    {
        // Arrange
<<<<<<< HEAD
        var existingEntity = await _dbContext.TeamMembers.Include(tm => tm.Category).FirstOrDefaultAsync()
=======
        var existingEntity = await Fixture.DbContext.TeamMembers.Include(tm => tm.Category).FirstOrDefaultAsync()
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/TeamMembers/{existingEntity!.Id}");
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
        var response = await Fixture.HttpClient.GetAsync($"api/TeamMembers/{-1}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
