using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetById;

[Collection("SharedIntegrationTests")]
public class GetTeamMemberByIdTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    public GetTeamMemberByIdTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task GetTeamMemberById_ShouldReturnOk()
    {
        // Arrange
        var existingEntity = await _dbContext.TeamMembers.Include(tm => tm.Category).OrderByDescending(tm => tm.Id).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        // Act
        var response = await _httpClient.GetAsync($"api/TeamMembers/{existingEntity!.Id}");
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
            () => Assert.Equal(existingEntity.Category.Name, responseContent.CategoryName),
            () => Assert.Equal(existingEntity.Description, responseContent.Description),
            () => Assert.Equal(existingEntity.Email, responseContent.Email),
            () => Assert.Equal(existingEntity.Status, responseContent.Status),
            () => Assert.Equal(existingEntity.Priority, responseContent.Priority));
    }

    [Fact]
    public async Task GetTeamMemberById_ShouldFail_NotFound()
    {
        var response = await _httpClient.GetAsync($"api/TeamMembers/{-1}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
