using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.Delete;

public class DeleteProgramTests : BaseTestClass
{
    public DeleteProgramTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteProgram_ShouldDeleteProgram()
    {
        DAL.Entities.Program? existingEntity = await Fixture.DbContext.Programs.FirstOrDefaultAsync();
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/Programs/{existingEntity!.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.Programs.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgram_ShouldNotDeleteProgram(int id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/Programs/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
