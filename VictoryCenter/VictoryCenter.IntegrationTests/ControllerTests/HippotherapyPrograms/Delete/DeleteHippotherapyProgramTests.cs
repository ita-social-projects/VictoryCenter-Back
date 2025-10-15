using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyPrograms.Delete;

public class DeleteHippotherapyProgramTests : BaseTestClass
{
    public DeleteHippotherapyProgramTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteProgram_ShouldDeleteProgram()
    {
        DAL.Entities.HippotherapyProgram? existingEntity = await Fixture.DbContext.HippotherapyPrograms.FirstOrDefaultAsync();
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/HippotherapyPrograms/{existingEntity!.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.HippotherapyPrograms.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgram_ShouldNotDeleteProgram(int id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/HippotherapyPrograms/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
