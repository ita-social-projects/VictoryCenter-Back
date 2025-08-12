using System.Text.Json;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.GetAll;

[Collection("SharedIntegrationTests")]
public class GetAllCategoriesTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    private readonly JsonSerializerOptions _jsonOptions;

    public GetAllCategoriesTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        var response = await _fixture.HttpClient.GetAsync("/api/categories");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(
            responseString,
            _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
