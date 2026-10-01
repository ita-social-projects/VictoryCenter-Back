using System.Net;
using System.Net.Http.Json;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.GetPublished;

public class GetPublishedFeedbackHistoriesTests : BaseTestClass
{
    private const string PublishedUrl = "/api/FeedbackHistories/published";

    public GetPublishedFeedbackHistoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublished_WithoutAuthorization_ShouldReturnOnlyPublishedHistoriesInPriorityOrder()
    {
        var second = await AddHistoryAsync("Second published story", Status.Published, priority: 2);
        var first = await AddHistoryAsync("First published story", Status.Published, priority: 1);
        var draft = await AddHistoryAsync("Draft story not shown", Status.Draft, priority: 3);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(PublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var histories = await response.Content.ReadFromJsonAsync<List<PublishedFeedbackHistoryDto>>();
        Assert.NotNull(histories);
        Assert.Equal([first.Id, second.Id], histories.Select(history => history.Id));
        Assert.DoesNotContain(histories, history => history.Id == draft.Id);
    }

    [Fact]
    public async Task GetPublished_ShouldReturnFullStoryAndImage()
    {
        var image = new Image { BlobName = "test-image.jpg", MimeType = "image/jpeg", Url = "test.com" };
        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();
        var fullStory = new string('a', 1000);
        await AddHistoryAsync("Story with image", Status.Published, priority: 1, story: fullStory, imageId: image.Id);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var histories = await anonymousClient.GetFromJsonAsync<List<PublishedFeedbackHistoryDto>>(PublishedUrl);

        var history = Assert.Single(histories!);
        Assert.Equal(fullStory, history.Story);
        Assert.NotNull(history.Image);
        Assert.Equal(image.Id, history.Image.Id);
        Assert.False(string.IsNullOrWhiteSpace(history.Image.Url));
    }

    [Fact]
    public async Task GetPublished_NoPublishedHistories_ShouldReturnEmptyList()
    {
        await AddHistoryAsync("Draft story not shown", Status.Draft, priority: 1);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(PublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var histories = await response.Content.ReadFromJsonAsync<List<PublishedFeedbackHistoryDto>>();
        Assert.Empty(histories!);
    }

    [Fact]
    public async Task GetAdminList_WithoutAuthorization_ShouldStillReturnUnauthorized()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/FeedbackHistories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<FeedbackHistory> AddHistoryAsync(
        string title,
        Status status,
        long priority,
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
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}
