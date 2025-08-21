using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteProgramTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public DeleteProgramTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteProgram_ShouldDeleteProgram()
    {
        DAL.Entities.Program? existingEntity = await _fixture.DbContext.Programs.FirstOrDefaultAsync();
        HttpResponseMessage response = await _fixture.HttpClient.DeleteAsync($"/api/Program/{existingEntity!.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _fixture.DbContext.Programs.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgram_ShouldNotDeleteProgram(int id)
    {
        HttpResponseMessage response = await _fixture.HttpClient.DeleteAsync($"/api/Program/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
