using System.Text.Json;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.GetAll;

[Collection("SharedIntegrationTests")]
public class GetAllCategoriesTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions;
    private readonly SeederManager _seederManager;

    public GetAllCategoriesTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _seederManager = fixture.SeederManager;
    }

    public async Task InitializeAsync()
    {
        await _seederManager.DisposeAllAsync();
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

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
