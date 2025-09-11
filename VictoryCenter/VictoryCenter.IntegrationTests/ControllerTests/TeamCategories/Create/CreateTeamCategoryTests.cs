using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamCategories.Create;

public class CreateTeamCategoryTests : BaseTestClass
{
    public CreateTeamCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task CreateCategory_ShouldCreateCategory(string? testDescription)
    {
        var createCategoryDto = new CreateTeamCategoryDto
        {
            Name = "Test Category",
            Description = testDescription,
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);

        var response = await Fixture.HttpClient.PostAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<TeamCategoryDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(createCategoryDto.Name, responseContent.Name);
        Assert.Equal(createCategoryDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateCategory_ShouldNotCreateCategory_InvalidName(string? testName)
    {
        var createCategoryDto = new CreateTeamCategoryDto
        {
            Name = testName!,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);

        var response = await Fixture.HttpClient.PostAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
