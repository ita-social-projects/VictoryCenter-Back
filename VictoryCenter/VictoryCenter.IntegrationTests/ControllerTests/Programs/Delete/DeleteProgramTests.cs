using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteProgramTests : IAsyncLifetime
{
    private IntegrationTestDbFixture _fixture;

    public DeleteProgramTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteProgram_ShouldDeleteProgram()
    {
        var existingEntity = await _fixture.DbContext.Programs.FirstOrDefaultAsync();
        var response = await _fixture.HttpClient.DeleteAsync($"/api/Program/{existingEntity!.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _fixture.DbContext.Programs.FirstOrDefaultAsync(e => e.Id == existingEntity!.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgram_ShouldNotDeleteProgram(int id)
    {
        var response = await _fixture.HttpClient.DeleteAsync($"/api/Program/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
