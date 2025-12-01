using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Search;

public class SearchTeamMemberTests : BaseTestClass
{
    public SearchTeamMemberTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchTeamMembers_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        string fullName = $"FirstName1 LastName1";

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/TeamMembers/search?fullname={fullName}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchTeamMembers_InvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        string fullName = $"";

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/TeamMembers/search?fullname={fullName}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
