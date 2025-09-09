using System.Net;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteCategoryTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    private readonly SeederManager _seederManager;

=======
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Delete;

public class DeleteCategoryTests : BaseTestClass
{
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e
    public DeleteCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
<<<<<<< HEAD
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException("SeederManager is not registered in the service collection.");
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
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
<<<<<<< HEAD
        var existingEntity = await _dbContext.Categories.OrderBy(c => c.Id).FirstOrDefaultAsync();
=======
        var existingEntity = await Fixture.DbContext.Categories.OrderBy(c => c.Id).LastOrDefaultAsync();
>>>>>>> dec19edb82ded7c9a85eabf645cb4e87878fa99e

        var response = await Fixture.HttpClient.DeleteAsync($"api/categories/{existingEntity!.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.Categories.FirstOrDefaultAsync(e => e.Id == existingEntity!.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteCategory_ShouldNotDeleteCategory_NotFound(long testId)
    {
        var response = await Fixture.HttpClient.DeleteAsync($"api/categories/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
