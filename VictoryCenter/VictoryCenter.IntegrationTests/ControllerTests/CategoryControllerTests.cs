using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests;

[Collection("SharedIntegrationTests")]
public class CategoryControllerTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;
    
    private readonly JsonSerializerOptions _jsonOptions;

    public CategoryControllerTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        
        _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetCategories_ShouldReturnAllCategories()
    {
        var response = await _httpClient.GetAsync("/api/categories/getCategories");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(responseString, 
            _jsonOptions);
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task CreateCategory_ShouldCreateCategory(string testDescription)
    {
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = testDescription,
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);
        
        var response = await _httpClient.PostAsync("api/categories/createCategory", new StringContent(
            serializedDto, Encoding.UTF8, @"application/json"));
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
    public async Task CreateCategory_ShouldNotCreateCategory_InvalidName(string testName)
    {
        var createCategoryDto = new CreateCategoryDto
        {
            Name = testName,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);
        
        var response = await _httpClient.PostAsync("api/category/createCategory", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        
        Assert.False(response.IsSuccessStatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task UpdateCategory_ShouldCreateCategory(string testDescription)
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync();
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = existingEntity!.Id,
            Name = "Test Category",
            Description = testDescription,
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);
        
        var response = await _httpClient.PutAsync("api/categories/updateCategory", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<CategoryDto>(responseString, _jsonOptions);
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(updateCategoryDto.Name, responseContent.Name);
        Assert.Equal(updateCategoryDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateCategory_ShouldNotUpdateCategory_InvalidName(string testName)
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync();
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = existingEntity!.Id,
            Name = testName,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);
        
        var response = await _httpClient.PutAsync("api/categories/updateCategory", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        
        Assert.False(response.IsSuccessStatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateCategory_ShouldNotUpdateCategory_NotFound(int testId)
    {
        var updateCategoryDto = new UpdateCategoryDto
        {
            Id = testId,
            Name = "Test Category",
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateCategoryDto);
        
        var response = await _httpClient.PutAsync("api/categories/updateCategory", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync();
        
        var response = await _httpClient.DeleteAsync($"api/categories/deleteCategory/{existingEntity!.Id}");
        
        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteCategory_ShouldNotDeleteCategory_NotFound(int testId)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/deleteCategory/{testId}");
        
        Assert.False(response.IsSuccessStatusCode);
    }
}
