using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteTeamMemberTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    public DeleteTeamMemberTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task DeleteTeamMember_ValidRequest_ShouldDeleteTeamMember()
    {
        var existingEntity = await _dbContext.TeamMembers.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No TeamMember entity exists in the database.");

        var response = await _httpClient.DeleteAsync($"/api/TeamMembers/{existingEntity.Id}");
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await _dbContext.TeamMembers.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteTeamMember_InvalidId_ShouldReturnNotFound(long testId)
    {
        var response = await _httpClient.DeleteAsync($"/api/TeamMembers/{testId}");
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
