using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteCategoryTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    public DeleteCategoryTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
        var existingEntity = await _dbContext.Categories.FirstOrDefaultAsync(e => e.Name == "TestForDelete");

        var response = await _httpClient.DeleteAsync($"api/categories/{existingEntity!.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _dbContext.Categories.FirstOrDefaultAsync(e => e.Id == existingEntity!.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteCategory_ShouldNotDeleteCategory_NotFound(long testId)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
