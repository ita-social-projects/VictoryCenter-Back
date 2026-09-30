using System.Net;
using System.Net.Http.Json;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.DTOs.Public.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.VideoReviews.GetPublished;

public class GetPublishedVideoReviewsTests : BaseTestClass
{
    private const string PublishedUrl = "/api/VideoReviews/published";

    public GetPublishedVideoReviewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublished_WithoutAuthorization_ShouldReturnOnlyPublishedNonArchivedVideosInPriorityOrder()
    {
        var second = await AddVideoReviewAsync("Second video", Status.Published, priority: 2);
        var first = await AddVideoReviewAsync("First video", Status.Published, priority: 1);
        var draft = await AddVideoReviewAsync("Draft video", Status.Draft, priority: 3);
        var archived = await AddVideoReviewAsync("Archived video", Status.Published, priority: 4, isArchived: true);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(PublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var videos = await response.Content.ReadFromJsonAsync<List<PublishedVideoReviewDto>>();
        Assert.NotNull(videos);
        Assert.Equal([first.Id, second.Id], videos.Select(video => video.Id));
        Assert.DoesNotContain(videos, video => video.Id == draft.Id);
        Assert.DoesNotContain(videos, video => video.Id == archived.Id);
        Assert.Equal(first.Link, videos[0].Link);
    }

    [Fact]
    public async Task GetPublished_VideoDeletedThroughAdminApi_ShouldNoLongerBeReturned()
    {
        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = "Video to be deleted",
                Link = "https://example.com/deleted-video",
                Status = Status.Published
            });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<VideoReviewDto>();
        using var anonymousClient = Fixture.Factory.CreateClient();

        var beforeDelete = await anonymousClient.GetFromJsonAsync<List<PublishedVideoReviewDto>>(PublishedUrl);
        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/VideoReviews/{created!.Id}");
        var afterDelete = await anonymousClient.GetFromJsonAsync<List<PublishedVideoReviewDto>>(PublishedUrl);

        Assert.Contains(beforeDelete!, video => video.Id == created.Id);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.DoesNotContain(afterDelete!, video => video.Id == created.Id);
    }

    [Fact]
    public async Task GetPublished_NoPublishedVideoReviews_ShouldReturnEmptyList()
    {
        await AddVideoReviewAsync("Draft video", Status.Draft, priority: 1);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(PublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var videos = await response.Content.ReadFromJsonAsync<List<PublishedVideoReviewDto>>();
        Assert.Empty(videos!);
    }

    private async Task<VideoReview> AddVideoReviewAsync(string title, Status status, long priority, bool isArchived = false)
    {
        var entity = new VideoReview
        {
            Title = title,
            Link = $"https://example.com/video-{priority}",
            Status = status,
            Priority = priority,
            IsArchived = isArchived,
            ArchivedAt = isArchived ? DateTimeOffset.UtcNow : null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.VideoReviews.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}
