using System.Net;
using System.Net.Http.Json;
using VictoryCenter.BLL.DTOs.Public.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackReviews.GetPublished;

public class GetPublishedFeedbackReviewsTests : BaseTestClass
{
    private const string PublishedUrl = "/api/FeedbackReviews/published";

    public GetPublishedFeedbackReviewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublished_WithoutAuthorization_ShouldReturnOnlyPublishedReviewsInPriorityOrder()
    {
        var second = await AddReviewAsync("Second author", Status.Published, priority: 2);
        var first = await AddReviewAsync("First author", Status.Published, priority: 1);
        var draft = await AddReviewAsync("Draft author", Status.Draft, priority: 3);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(PublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var reviews = await response.Content.ReadFromJsonAsync<List<PublishedFeedbackReviewDto>>();
        Assert.NotNull(reviews);
        Assert.Equal([first.Id, second.Id], reviews.Select(review => review.Id));
        Assert.DoesNotContain(reviews, review => review.Id == draft.Id);
        Assert.Equal("First author", reviews[0].AuthorName);
        Assert.Equal(first.Text, reviews[0].Text);
    }

    [Fact]
    public async Task GetPublished_NoPublishedReviews_ShouldReturnEmptyList()
    {
        await AddReviewAsync("Draft author", Status.Draft, priority: 1);

        using var anonymousClient = Fixture.Factory.CreateClient();
        var response = await anonymousClient.GetAsync(PublishedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var reviews = await response.Content.ReadFromJsonAsync<List<PublishedFeedbackReviewDto>>();
        Assert.Empty(reviews!);
    }

    [Fact]
    public async Task GetAdminList_WithoutAuthorization_ShouldStillReturnUnauthorized()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/FeedbackReviews");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<FeedbackReview> AddReviewAsync(string authorName, Status status, long priority)
    {
        var entity = new FeedbackReview
        {
            AuthorName = authorName,
            Text = $"Review text written by {authorName}.",
            Status = status,
            Priority = priority,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.FeedbackReviews.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}
