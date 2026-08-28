using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
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
                Link = "  https://example.com/video  "
            });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<VideoReviewDto>();
        Assert.NotNull(created);
        Assert.Equal(title, created.Title);
        Assert.Equal("https://example.com/video", created.Link);

        var getResponse = await Fixture.HttpClient.GetAsync("/api/VideoReviews");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var videoReviews = await getResponse.Content.ReadFromJsonAsync<List<VideoReviewDto>>();
        Assert.Contains(videoReviews!, item => item.Id == created.Id);

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/VideoReviews/{created.Id}",
            new UpdateVideoReviewDto
            {
                Title = updatedTitle,
                Link = "https://example.com/updated"
            });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<VideoReviewDto>();
        Assert.NotNull(updated);
        Assert.Equal(updatedTitle, updated.Title);
        Assert.Equal("https://example.com/updated", updated.Link);

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/VideoReviews/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deletedId = await deleteResponse.Content.ReadFromJsonAsync<long>();
        Assert.Equal(created.Id, deletedId);
        Assert.False(await Fixture.DbContext.VideoReviews
            .AsNoTracking()
            .AnyAsync(item => item.Id == created.Id));
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
}
