using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.VideoReviews;

public class VideoReviewManagementTests : BaseTestClass
{
    public VideoReviewManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldSupportAuthorizedCrud()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var title = $"Title{suffix}";
        var updatedTitle = $"Updated{suffix}";

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = $"  {title}  ",
                Link = "  https://example.com/video  ",
                Status = Status.Draft
            });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<VideoReviewDto>();
        Assert.NotNull(created);
        Assert.Equal(title, created.Title);
        Assert.Equal("https://example.com/video", created.Link);
        Assert.Equal(Status.Draft, created.Status);
        Assert.True(created.Priority >= 0);

        var getResponse = await Fixture.HttpClient.GetAsync("/api/VideoReviews");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var videoReviews = await getResponse.Content.ReadFromJsonAsync<List<VideoReviewDto>>();
        Assert.Contains(videoReviews!, item => item.Id == created.Id);

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/VideoReviews/{created.Id}",
            new UpdateVideoReviewDto
            {
                Title = updatedTitle,
                Link = "https://example.com/updated",
                Status = Status.Published
            });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<VideoReviewDto>();
        Assert.NotNull(updated);
        Assert.Equal(updatedTitle, updated.Title);
        Assert.Equal("https://example.com/updated", updated.Link);
        Assert.Equal(Status.Published, updated.Status);
        Assert.Equal(created.Priority, updated.Priority);

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/VideoReviews/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deletedId = await deleteResponse.Content.ReadFromJsonAsync<long>();
        Assert.Equal(created.Id, deletedId);
        Assert.False(await Fixture.DbContext.VideoReviews
            .AsNoTracking()
            .AnyAsync(item => item.Id == created.Id));
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldAssignIncreasingPriorityOnCreate()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var first = await CreateVideoReviewAsync($"First{suffix}");
        var second = await CreateVideoReviewAsync($"Second{suffix}");
        var third = await CreateVideoReviewAsync($"Third{suffix}");

        Assert.True(second.Priority > first.Priority);
        Assert.True(third.Priority > second.Priority);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldRenumberPriorityAfterDeletingMiddleItem()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var first = await CreateVideoReviewAsync($"First{suffix}");
        var middle = await CreateVideoReviewAsync($"Middle{suffix}");
        var last = await CreateVideoReviewAsync($"Last{suffix}");

        Assert.Equal(first.Priority + 1, middle.Priority);
        Assert.Equal(first.Priority + 2, last.Priority);

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/VideoReviews/{middle.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await Fixture.HttpClient.GetAsync("/api/VideoReviews");
        var videoReviews = await getResponse.Content.ReadFromJsonAsync<List<VideoReviewDto>>();

        var firstAfterDelete = videoReviews!.Single(item => item.Id == first.Id);
        var lastAfterDelete = videoReviews!.Single(item => item.Id == last.Id);

        Assert.Equal(firstAfterDelete.Priority + 1, lastAfterDelete.Priority);
        Assert.DoesNotContain(videoReviews!, item => item.Id == middle.Id);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldReturnNotFound_ForMissingVideoReview()
    {
        const long missingId = long.MaxValue;

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/VideoReviews/{missingId}",
            new UpdateVideoReviewDto { Title = "Missing title", Link = "https://example.com/missing" });
        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/VideoReviews/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldReturnBadRequest_ForInvalidLink()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = "Valid title",
                Link = "not-a-valid-link"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldReturnBadRequest_ForMissingLink()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = "Valid title",
                Link = string.Empty
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldReturnBadRequest_ForEmptyTitle()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = string.Empty,
                Link = "https://example.com/video"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldReturnBadRequest_ForTitleShorterThanMinLength()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = "abcd",
                Link = "https://example.com/video"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VideoReviewManagement_ShouldReturnBadRequest_ForInvalidStatus()
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = "Valid title",
                Link = "https://example.com/video",
                Status = (Status)999
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VideoReviewMutations_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var getResponse = await anonymousClient.GetAsync("/api/VideoReviews");
        var createResponse = await anonymousClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto { Title = "Unauthorized", Link = "https://example.com/video" });

        Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);
    }

    private async Task<VideoReviewDto> CreateVideoReviewAsync(string title)
    {
        var response = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/VideoReviews",
            new CreateVideoReviewDto
            {
                Title = title,
                Link = "https://example.com/video"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<VideoReviewDto>();
        Assert.NotNull(created);

        return created;
    }
}
