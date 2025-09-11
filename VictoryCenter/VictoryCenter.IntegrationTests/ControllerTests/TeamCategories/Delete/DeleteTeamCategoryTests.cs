using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamCategories.Delete;

public class DeleteTeamCategoryTests : BaseTestClass
{
    public DeleteTeamCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
        var existingEntity = await Fixture.DbContext.TeamCategories.OrderBy(c => c.Id).LastOrDefaultAsync();

        var response = await Fixture.HttpClient.DeleteAsync($"api/categories/{existingEntity!.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync(e => e.Id == existingEntity!.Id));
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
