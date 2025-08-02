using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteProgramCategoryTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly SeederManager _seederManager;

    public DeleteProgramCategoryTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException(
            "SeederManager is not registered in the service collection.");
    }

    public async Task InitializeAsync()
    {
        await _seederManager.DisposeAllAsync();
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteProgramCategory_ShouldDeleteProgramCategory()
    {
        var existingEntity = await _dbContext.ProgramCategories.FirstOrDefaultAsync();

        var response = await _httpClient.DeleteAsync($"/api/ProgramCategory/5");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _dbContext.ProgramCategories.FirstOrDefaultAsync(e => e.Id == 5));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgramCategory_ShouldNotDeleteProgramCategory(int testId)
    {
        var response = await _httpClient.DeleteAsync($"/api/ProgramCategory/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
