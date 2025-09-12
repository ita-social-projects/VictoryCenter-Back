using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetById;

public class GetTeamMemberByIdTests : BaseTestClass
{
    public GetTeamMemberByIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetTeamMemberById_ShouldReturnOk()
    {
        // Arrange
        var existingEntity = await Fixture.DbContext.TeamMembers.Include(tm => tm.Category).FirstOrDefaultAsync()
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
