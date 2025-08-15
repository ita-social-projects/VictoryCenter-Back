using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.Delete;

public class DeleteFaqQuestionTests : BaseTestClass
{
    public DeleteFaqQuestionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteFaqQuestion_ValidRequest_ShouldDelete()
    {
        var existingEntity = await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No FaqQuestion entity exists in the database.");

        var response = await Fixture.HttpClient.DeleteAsync(new Uri($"/api/faq/{existingEntity.Id}", UriKind.Relative));
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteFaqQuestion_InvalidId_ShouldReturnNotFound(long testId)
    {
        var response = await Fixture.HttpClient.DeleteAsync(new Uri($"/api/faq/{testId}", UriKind.Relative));
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
