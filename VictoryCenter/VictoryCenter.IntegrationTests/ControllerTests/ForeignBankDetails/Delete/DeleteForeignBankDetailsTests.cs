using System.Net;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.IntegrationTests.ControllerTests.ForeignBankDetails.Delete;

public class DeleteForeignBankDetailsTests : BaseTestClass
{
    public DeleteForeignBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ForeignBankDetails_ShouldDelete()
    {
        var existingEntity = await Fixture.DbContext.ForeignBankDetails.FirstOrDefaultAsync();
        Assert.NotNull(existingEntity);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/ForeignBankDetails/{existingEntity.Id}");
        response.EnsureSuccessStatusCode();

        Assert.Null(await Fixture.DbContext.ForeignBankDetails.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ForeignBankDetails_ShouldNotDelete_NotFound(long id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/ForeignBankDetails/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
