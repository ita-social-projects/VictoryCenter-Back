using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.Delete;

public class DeleteFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public DeleteFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteFeedbackHistory_ValidRequest_ShouldDeleteEntity()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var response = await Fixture.HttpClient.DeleteAsync($"{BaseUrl}/{existingEntity.Id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await Fixture.DbContext.FeedbackHistories.FirstOrDefaultAsync(e => e.Id == existingEntity.Id));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteFeedbackHistory_InvalidId_ShouldReturnNotFound(long testId)
    {
        var response = await Fixture.HttpClient.DeleteAsync($"{BaseUrl}/{testId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<FeedbackHistory> CreateTestFeedbackHistoryAsync()
    {
        var entity = new FeedbackHistory
        {
            Title = "Title For Delete Test",
            Story = "Story content for delete test that meets all length requirements.",
            ImageId = null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}