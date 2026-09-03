using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class DeleteFeedbackReviewTests
{
    private readonly Mock<IFeedbackReviewsRepository> _repository = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IReorderService> _reorderService = new();

    public DeleteFeedbackReviewTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.FeedbackReviewsRepository)
            .Returns(_repository.Object);

        _repositoryWrapper
            .Setup(wrapper => wrapper.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _reorderService
            .Setup(service => service.RenumberPriorityAsync<FeedbackReview>(null))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ExistingReview_DeletesAndReturnsId()
    {
        var review = Review(10);
        SetupReview(review);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = CreateHandler();

        var result = await handler.Handle(new DeleteFeedbackReviewCommand(review.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(review.Id, result.Value);
        _repository.Verify(repository => repository.Delete(review), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingReview_RenumbersPriority()
    {
        var review = Review(10);
        SetupReview(review);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(1);
        var handler = CreateHandler();

        await handler.Handle(new DeleteFeedbackReviewCommand(review.Id), CancellationToken.None);

        _reorderService.Verify(
            service => service.RenumberPriorityAsync<FeedbackReview>(null),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReviewDoesNotExist_ReturnsNotFound()
    {
        const long reviewId = 10;
        SetupReview(null);
        var handler = CreateHandler();

        var result = await handler.Handle(new DeleteFeedbackReviewCommand(reviewId), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(reviewId, typeof(FeedbackReview)),
            result.Errors[0].Message);
        _repository.Verify(repository => repository.Delete(It.IsAny<FeedbackReview>()), Times.Never);
        _reorderService.Verify(
            service => service.RenumberPriorityAsync<FeedbackReview>(null),
            Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailure()
    {
        var review = Review(10);
        SetupReview(review);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(0);
        var handler = CreateHandler();

        var result = await handler.Handle(new DeleteFeedbackReviewCommand(review.Id), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ReturnsDatabaseFailure()
    {
        var review = Review(10);
        SetupReview(review);
        _repositoryWrapper.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = CreateHandler();

        var result = await handler.Handle(new DeleteFeedbackReviewCommand(review.Id), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(FeedbackReview)),
            result.Errors[0].Message);
    }

    private void SetupReview(FeedbackReview? review)
    {
        _repository
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(review);
    }

    private DeleteFeedbackReviewHandler CreateHandler() =>
        new(_repositoryWrapper.Object, _reorderService.Object);

    private static FeedbackReview Review(long id) => new()
    {
        Id = id,
        AuthorName = "Anastasiia",
        Text = "Very happy",
        Priority = 0,
        CreatedAt = DateTimeOffset.UtcNow
    };
}
