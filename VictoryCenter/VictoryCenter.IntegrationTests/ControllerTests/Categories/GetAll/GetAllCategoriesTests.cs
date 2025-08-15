using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.GetAll;

public class GetAllCategoriesTests : BaseTestClass
{
    public GetAllCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        var response = await Fixture.HttpClient.GetAsync("/api/categories");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(
            responseString,
            JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
