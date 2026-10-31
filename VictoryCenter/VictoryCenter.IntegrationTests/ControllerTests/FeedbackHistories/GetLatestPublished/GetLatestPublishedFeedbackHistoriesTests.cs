using System.Net;
using System.Net.Http.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.GetLatestPublished;

public class GetLatestPublishedFeedbackHistoriesTests : BaseTestClass
{
    private const string LatestPublishedUrl = "/api/FeedbackHistories/published/latest";

    public GetLatestPublishedFeedbackHistoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetLatestPublished_WithoutAuthorization_ShouldReturnOnlyFourNewestByPublishedAtIgnoringPriority()
    {
        var now = DateTimeOffset.UtcNow;

        var oldest = await AddHistoryAsync("Oldest published", Status.Published, priority: 1, publishedAt: now.AddDays(-10));
        var fourth = await AddHistoryAsync("Fourth newest", Status.Published, priority: 100, publishedAt: now.AddDays(-4));
        var third = await AddHistoryAsync("Third newest", Status.Published, priority: 50, publishedAt: now.AddDays(-3));
        var second = await AddHistoryAsync("Second newest", Status.Published, priority: 2, publishedAt: now.AddDays(-2));
        var first = await AddHistoryAsync("Newest", Status.Published, priority: 999, publishedAt: now.AddDays(-1));

        await AddHistoryAsync("Published without PublishedAt", Status.Published, priority: 3, publishedAt: null);
        await AddHistoryAsync("Draft with date", Status.Draft, priority: 4, publishedAt: now);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(LatestPublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var histories = await response.Content.ReadFromJsonAsync<List<PublishedFeedbackHistoryDto>>();
        Assert.NotNull(histories);

        Assert.Equal(FeedbackHistoryConstants.LatestPublishedFeedbackHistoryCount, histories.Count);
        Assert.Equal([first.Id, second.Id, third.Id, fourth.Id], histories.Select(history => history.Id));
        Assert.DoesNotContain(histories, history => history.Id == oldest.Id);
    }

    [Fact]
    public async Task GetLatestPublished_NoEligibleHistories_ShouldReturnEmptyList()
    {
        var now = DateTimeOffset.UtcNow;
        await AddHistoryAsync("Draft", Status.Draft, priority: 1, publishedAt: now);
        await AddHistoryAsync("Published without date", Status.Published, priority: 2, publishedAt: null);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(LatestPublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var histories = await response.Content.ReadFromJsonAsync<List<PublishedFeedbackHistoryDto>>();
        Assert.NotNull(histories);
        Assert.Empty(histories);
    }

    private async Task<FeedbackHistory> AddHistoryAsync(
        string title,
        Status status,
        long priority,
        DateTimeOffset? publishedAt,
        string story = "Story content for the public page.",
        long? imageId = null)
    {
        var entity = new FeedbackHistory
        {
            Title = title,
            Story = story,
            ImageId = imageId,
            Status = status,
            Priority = priority,
            PublishedAt = publishedAt,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();

        return entity;
    }
}
