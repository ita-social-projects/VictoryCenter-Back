using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteCategoryTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public DeleteCategoryTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
        var existingEntity = await _fixture.DbContext.Categories.OrderBy(c => c.Id).LastOrDefaultAsync();

        var response = await _fixture.HttpClient.DeleteAsync($"api/categories/{existingEntity!.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _fixture.DbContext.Categories.FirstOrDefaultAsync(e => e.Id == existingEntity!.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteCategory_ShouldNotDeleteCategory_NotFound(long testId)
    {
        var response = await _fixture.HttpClient.DeleteAsync($"api/categories/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
