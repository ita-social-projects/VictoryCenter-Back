using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamCategories.GetAll;

public class GetAllTeamCategoriesTests : BaseTestClass
{
    public GetAllTeamCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        var response = await Fixture.HttpClient.GetAsync("/api/teamcategories");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<TeamCategoryDto>>(
            responseString,
            JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
