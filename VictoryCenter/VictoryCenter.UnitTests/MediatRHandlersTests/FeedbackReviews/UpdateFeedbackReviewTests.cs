using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class UpdateFeedbackReviewTests
{
    private static readonly DateTimeOffset OriginalCreatedAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IFeedbackReviewsRepository> _repository = new();

    public UpdateFeedbackReviewTests()
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
    public async Task Handle_ExistingReview_UpdatesFieldsAndReturnsDto()
    {
        var review = Review(10);
        SetupReview(review, saveChanges: 1);
        var handler = CreateHandler();

        var result = await handler.Handle(Command(10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated author", review.AuthorName);
        Assert.Equal("Updated review text", review.Text);
        Assert.Equal(Status.Published, review.Status);
    }

    [Fact]
    public async Task Handle_ExistingReview_TrimsTextFields()
    {
        var review = Review(10);
        SetupReview(review, saveChanges: 1);
        var handler = CreateHandler();

        await handler.Handle(
            Command(10, new UpdateFeedbackReviewDto
            {
                AuthorName = "  Updated author  ",
                Text = "  Updated review text  ",
                Status = Status.Published
            }),
            CancellationToken.None);

        Assert.Equal("Updated author", review.AuthorName);
        Assert.Equal("Updated review text", review.Text);
    }

    [Fact]
    public async Task Handle_ExistingReview_DoesNotChangePriorityOrCreatedAt()
    {
        var review = Review(10);
        SetupReview(review, saveChanges: 1);
        var handler = CreateHandler();

        await handler.Handle(Command(10), CancellationToken.None);

        Assert.Equal(7, review.Priority);
        Assert.Equal(OriginalCreatedAt, review.CreatedAt);
    }

    [Fact]
    public async Task Handle_ReviewDoesNotExist_ReturnsNotFound()
    {
        const long reviewId = 10;
        SetupReview(null);
        var handler = CreateHandler();

        var result = await handler.Handle(Command(reviewId), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(reviewId, typeof(FeedbackReview)),
            result.Errors[0].Message);
        _repositoryWrapper.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailure()
    {
        var review = Review(10);
        SetupReview(review, saveChanges: 0);
        var handler = CreateHandler();

        var result = await handler.Handle(Command(10), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ReturnsDatabaseFailure()
    {
        var review = Review(10);
        SetupReview(review);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = CreateHandler();

        var result = await handler.Handle(Command(10), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    private void SetupReview(FeedbackReview? review, int saveChanges = 1)
    {
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(review);

        _repositoryWrapper
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ReturnsAsync(saveChanges);
    }

    private UpdateFeedbackReviewHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);

    private static UpdateFeedbackReviewCommand Command(long id, UpdateFeedbackReviewDto? dto = null) =>
        new(id, dto ?? new UpdateFeedbackReviewDto
        {
            AuthorName = "Updated author",
            Text = "Updated review text",
            Status = Status.Published
        });

    private static FeedbackReview Review(long id) => new()
    {
        Id = id,
        AuthorName = "Original author",
        Text = "Original review text",
        Status = Status.Draft,
        Priority = 7,
        CreatedAt = OriginalCreatedAt
    };
}
