using System.Net;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.IntegrationTests.ControllerTests.CorrespondentBankDetails.Delete;

public class DeleteCorrespondentBankDetailsTests : BaseTestClass
{
    public DeleteCorrespondentBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CorrespondentBankDetails_ShouldDelete()
    {
        var existingEntity = await Fixture.DbContext.CorrespondentBankDetails.FirstOrDefaultAsync();
        Assert.NotNull(existingEntity);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/CorrespondentBankDetails/{existingEntity.Id}");

        response.EnsureSuccessStatusCode();

        Assert.Null(await Fixture.DbContext.CorrespondentBankDetails.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task CorrespondentBankDetails_ShouldNotDelete_NotFound(long id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"/api/CorrespondentBankDetails/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
