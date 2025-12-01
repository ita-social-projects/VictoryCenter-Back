using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyPrograms.Search;

public class SearchHippotherapyProgramsTests : BaseTestClass
{
    public SearchHippotherapyProgramsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchHippotherapyPrograms_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        string searchQuery = "Program";

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/HippotherapyPrograms/search?searchQuery={searchQuery}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchHippotherapyPrograms_InvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        string searchQuery = "";

        // Act
        var response = await Fixture.HttpClient.GetAsync($"api/HippotherapyPrograms/search?searchQuery={searchQuery}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
