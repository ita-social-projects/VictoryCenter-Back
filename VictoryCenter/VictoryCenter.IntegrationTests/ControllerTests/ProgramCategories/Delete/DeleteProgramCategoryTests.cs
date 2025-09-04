using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.Delete;

public class DeleteProgramCategoryTests : BaseTestClass
{
    public DeleteProgramCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteProgramCategory_ShouldDeleteProgramCategory()
    {
        ProgramCategory? existingEntity = await Fixture.DbContext.ProgramCategories
            .FirstOrDefaultAsync(e => e.Id == 1);
        Assert.NotNull(existingEntity);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/ProgramCategory/{existingEntity.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.ProgramCategories.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgramCategory_ShouldNotDeleteProgramCategory(int testId)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/ProgramCategory/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
