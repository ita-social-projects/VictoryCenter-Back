using System.Text.Json;
<<<<<<< HEAD
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

=======
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.GetAll;

public class GetAllCategoriesTests : BaseTestClass
{
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    public GetAllCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
<<<<<<< HEAD
        _httpClient = fixture.HttpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _seederManager = fixture.SeederManager;
=======
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
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
