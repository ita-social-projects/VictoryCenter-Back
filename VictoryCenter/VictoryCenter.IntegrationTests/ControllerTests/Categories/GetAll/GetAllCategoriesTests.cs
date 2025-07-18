using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.GetAll;

[Collection("SharedIntegrationTests")]
public class GetAllCategoriesTests
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions;

    public GetAllCategoriesTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        var response = await _httpClient.GetAsync("/api/categories");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(
            responseString,
            _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
