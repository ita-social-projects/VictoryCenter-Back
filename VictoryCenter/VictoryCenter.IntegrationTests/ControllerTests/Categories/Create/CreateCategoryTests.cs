using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Create;

[Collection("SharedIntegrationTests")]
public class CreateCategoryTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateCategoryTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task CreateCategory_ShouldCreateCategory(string? testDescription)
    {
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = testDescription,
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);

        var response = await _fixture.HttpClient.PostAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<CategoryDto>(responseString, _jsonOptions);

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
        var createCategoryDto = new CreateCategoryDto
        {
            Name = testName!,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);

        var response = await _fixture.HttpClient.PostAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
