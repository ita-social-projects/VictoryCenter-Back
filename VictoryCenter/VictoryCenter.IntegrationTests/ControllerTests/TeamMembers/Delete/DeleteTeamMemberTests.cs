using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Delete;

public class DeleteTeamMemberTests : BaseTestClass
{
    public DeleteTeamMemberTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

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
