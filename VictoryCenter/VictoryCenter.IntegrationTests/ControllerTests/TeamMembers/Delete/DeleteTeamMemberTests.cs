using System.Net;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteTeamMemberTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly SeederManager _seederManager;

=======
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Delete;

public class DeleteTeamMemberTests : BaseTestClass
{
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    public DeleteTeamMemberTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
<<<<<<< HEAD
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
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
    public async Task DeleteTeamMember_ValidRequest_ShouldDeleteTeamMember()
    {
        var existingEntity = await Fixture.DbContext.TeamMembers.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No TeamMember entity exists in the database.");

        var response = await Fixture.HttpClient.DeleteAsync($"/api/TeamMembers/{existingEntity.Id}");
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await Fixture.DbContext.TeamMembers.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteTeamMember_InvalidId_ShouldReturnNotFound(long testId)
    {
        var response = await Fixture.HttpClient.DeleteAsync($"/api/TeamMembers/{testId}");
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
