using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyPrograms.Delete;

public class DeleteHypotherapyProgramTests : BaseTestClass
{
    public DeleteHypotherapyProgramTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteProgram_ShouldDeleteProgram()
    {
        DAL.Entities.HypotherapyProgram? existingEntity = await Fixture.DbContext.HypotherapyPrograms.FirstOrDefaultAsync();
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/HypotherapyPrograms/{existingEntity!.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.HypotherapyPrograms.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgram_ShouldNotDeleteProgram(int id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/HypotherapyPrograms/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
