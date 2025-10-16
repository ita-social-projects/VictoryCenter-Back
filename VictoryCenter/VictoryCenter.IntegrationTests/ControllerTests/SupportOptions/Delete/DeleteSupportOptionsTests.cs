using System.Net;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.IntegrationTests.ControllerTests.SupportOptions.Delete;
public class DeleteSupportOptionsTests : BaseTestClass
{
    public DeleteSupportOptionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SupportOptions_ShouldDelete()
    {
        var existingEntity = await Fixture.DbContext.SupportOptions.FirstOrDefaultAsync();
        Assert.NotNull(existingEntity);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/SupportOptions/{existingEntity.Id}");
        response.EnsureSuccessStatusCode();

        Assert.Null(await Fixture.DbContext.SupportOptions.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task SupportOptions_ShouldNotDelete_NotFound(long id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/SupportOptions/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
