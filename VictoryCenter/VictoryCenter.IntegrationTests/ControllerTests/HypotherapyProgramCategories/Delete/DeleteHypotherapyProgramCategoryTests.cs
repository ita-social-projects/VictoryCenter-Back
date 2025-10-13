using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyProgramCategories.Delete;

public class DeleteHypotherapyProgramCategoryTests : BaseTestClass
{
    public DeleteHypotherapyProgramCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteProgramCategory_ShouldDeleteProgramCategory()
    {
        HippotherapyProgramCategory? existingEntity = await Fixture.DbContext.HypotherapyProgramCategories
            .FirstOrDefaultAsync(e => e.Id == 1);
        Assert.NotNull(existingEntity);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/HypotherapyProgramCategory/{existingEntity.Id}");
        response.EnsureSuccessStatusCode();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.HypotherapyProgramCategories.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteProgramCategory_ShouldNotDeleteProgramCategory(int testId)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/HypotherapyProgramCategory/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
