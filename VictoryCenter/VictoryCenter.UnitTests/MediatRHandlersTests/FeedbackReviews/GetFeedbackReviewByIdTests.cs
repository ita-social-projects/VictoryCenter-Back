using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class GetFeedbackReviewByIdTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IFeedbackReviewsRepository> _repository = new();

    public GetFeedbackReviewByIdTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.FeedbackReviewsRepository)
            .Returns(_repository.Object);

        _mapper
            .Setup(mapper => mapper.Map<FeedbackReviewDto>(It.IsAny<FeedbackReview>()))
            .Returns((FeedbackReview review) => new FeedbackReviewDto
            {
                Id = review.Id,
                AuthorName = review.AuthorName,
                Text = review.Text,
                Status = review.Status,
                Priority = review.Priority,
                CreatedAt = review.CreatedAt
            });
    }

    [Fact]
    public async Task Handle_ExistingReview_ReturnsDto()
    {
        var review = Review(10);
        SetupReview(review);
        var handler = CreateHandler();

        var result = await handler.Handle(new GetFeedbackReviewByIdQuery(10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.Id);
        Assert.Equal("Anastasiia", result.Value.AuthorName);
    }

    [Fact]
    public async Task Handle_ExistingReview_UsesReadOnlyQuery()
    {
        SetupReview(Review(10));
        var handler = CreateHandler();

        await handler.Handle(new GetFeedbackReviewByIdQuery(10), CancellationToken.None);

        _repository.Verify(
            repository => repository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<FeedbackReview>>(options =>
                    options.AsNoTracking && options.Filter != null)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReviewDoesNotExist_ReturnsNotFound()
    {
        const long reviewId = 999;
        SetupReview(null);
        var handler = CreateHandler();

        var result = await handler.Handle(new GetFeedbackReviewByIdQuery(reviewId), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(reviewId, typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    private void SetupReview(FeedbackReview? review)
    {
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(review);
    }

    private GetFeedbackReviewByIdHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);

    private static FeedbackReview Review(long id) => new()
    {
        Id = id,
        AuthorName = "Anastasiia",
        Text = "Very happy with the therapy",
        Status = Status.Published,
        Priority = 0,
        CreatedAt = DateTimeOffset.UtcNow
    };
}
