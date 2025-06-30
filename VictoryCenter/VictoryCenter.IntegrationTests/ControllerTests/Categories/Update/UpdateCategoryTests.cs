using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Update;

[Collection("SharedIntegrationTests")]
public class UpdateCategoryTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateCategoryTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task UpdateCategory_ShouldUpdateCategory(string? testDescription)
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync();
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = existingEntity!.Id,
            Name = "Test Category",
            Description = testDescription,
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);

        var response = await _httpClient.PutAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<CategoryDto>(responseString, _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateCategoryDto.Name, responseContent.Name);
        Assert.Equal(updateCategoryDto.Description, responseContent.Description);
    }

    [Fact]
    public async Task UpdateCategory_WhenSameInput_ShouldUpdateCategory()
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync();
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = existingEntity!.Id,
            Name = existingEntity.Name,
            Description = existingEntity.Description,
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);

        var response = await _httpClient.PutAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<CategoryDto>(responseString, _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateCategoryDto.Name, responseContent.Name);
        Assert.Equal(updateCategoryDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateCategory_WhenNameInvalid_ShouldNotUpdateCategory(string? testName)
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync();
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = existingEntity!.Id,
            Name = testName,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);

        var response = await _httpClient.PutAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateCategory_WhenCategoryNotFound_ShouldNotUpdateCategory(long testId)
    {
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = testId,
            Name = "Test Category",
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);

        var response = await _httpClient.PutAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
