using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.UahBankDetails.Delete;

public class DeleteUahBankDetailsTests : BaseTestClass
{
    public DeleteUahBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UahBankDetails_ShouldDelete()
    {
        var existingEntity = await Fixture.DbContext.UahBankDetails.FirstOrDefaultAsync();
        Assert.NotNull(existingEntity);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/UahBankDetails/{existingEntity.Id}");
        response.EnsureSuccessStatusCode();

        Assert.Null(await Fixture.DbContext.UahBankDetails.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UahBankDetails_ShouldNotDelete_NotFound(long id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/UahBankDetails/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
