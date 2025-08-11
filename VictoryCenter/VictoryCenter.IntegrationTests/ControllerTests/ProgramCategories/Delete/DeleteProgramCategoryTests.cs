using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteProgramCategoryTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public DeleteProgramCategoryTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteProgramCategory_ShouldDeleteProgramCategory()
    {
        var existingEntity = await _fixture.DbContext.ProgramCategories
            .FirstOrDefaultAsync(e => e.Id == 1);
        Assert.NotNull(existingEntity);

        var response = await _fixture.HttpClient.DeleteAsync($"/api/ProgramCategory/{existingEntity.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _fixture.DbContext.ProgramCategories.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgramCategory_ShouldNotDeleteProgramCategory(int testId)
    {
        var response = await _fixture.HttpClient.DeleteAsync($"/api/ProgramCategory/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
