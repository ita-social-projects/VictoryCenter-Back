using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteTeamMemberTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public DeleteTeamMemberTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteTeamMember_ValidRequest_ShouldDeleteTeamMember()
    {
        var existingEntity = await _fixture.DbContext.TeamMembers.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No TeamMember entity exists in the database.");

        var response = await _fixture.HttpClient.DeleteAsync($"/api/TeamMembers/{existingEntity.Id}");
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await _fixture.DbContext.TeamMembers.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteTeamMember_InvalidId_ShouldReturnNotFound(long testId)
    {
        var response = await _fixture.HttpClient.DeleteAsync($"/api/TeamMembers/{testId}");
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
