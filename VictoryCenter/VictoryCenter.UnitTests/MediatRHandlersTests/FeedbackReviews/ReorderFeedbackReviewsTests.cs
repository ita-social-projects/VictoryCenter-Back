using System.Linq.Expressions;
using FluentValidation;
using MediatR;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.FeedbackReviews;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class ReorderFeedbackReviewsTests
{
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderFeedbackReviewsCommand> _validator;

    private readonly ReorderFeedbackReviewsDto _testValidReorderDto = new()
    {
        OrderedIds = [4, 2, 5, 1, 3]
    };

    public ReorderFeedbackReviewsTests()
    {
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderFeedbackReviewsValidator();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReorderReviews()
    {
        SetupReorderService();

        var handler = new ReorderFeedbackReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackReviewsCommand(_testValidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);

        _mockReorderService.Verify(
            x => x.SwapElementsAsync(
                It.Is<List<long>>(ids => ids.SequenceEqual(_testValidReorderDto.OrderedIds)),
                It.IsAny<Expression<Func<FeedbackReview, long>>>(),
                It.IsAny<Expression<Func<FeedbackReview, bool>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyOrderedIds_ShouldReturnError()
    {
        var invalidReorderDto = new ReorderFeedbackReviewsDto
        {
            OrderedIds = []
        };

        SetupReorderService();
        var handler = new ReorderFeedbackReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackReviewsCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderFeedbackReviewsDto.OrderedIds)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DuplicateIdsInOrderedIds_ShouldReturnError()
    {
        var invalidReorderDto = new ReorderFeedbackReviewsDto
        {
            OrderedIds = [1, 2, 2, 3]
        };

        SetupReorderService();
        var handler = new ReorderFeedbackReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackReviewsCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(ReorderFeedbackReviewsDto.OrderedIds)),
            result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_InvalidIdInOrderedIds_ShouldReturnError(long invalidId)
    {
        var invalidReorderDto = new ReorderFeedbackReviewsDto
        {
            OrderedIds = [1, 2, invalidId]
        };

        SetupReorderService();
        var handler = new ReorderFeedbackReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackReviewsCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBePositive(
                $"Each {nameof(ReorderFeedbackReviewsDto.OrderedIds)} element"),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderServiceThrowsReorderException_ShouldReturnReorderError()
    {
        var reorderExceptionMessage = "Reorder operation failed";

        _mockReorderService.Setup(x => x.SwapElementsAsync<FeedbackReview>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<FeedbackReview, long>>>(),
                It.IsAny<Expression<Func<FeedbackReview, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderExceptionMessage));

        var handler = new ReorderFeedbackReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackReviewsCommand(_testValidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderExceptionMessage), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(x => x.SwapElementsAsync<FeedbackReview>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<FeedbackReview, long>>>(),
                It.IsAny<Expression<Func<FeedbackReview, bool>>>()))
            .Returns(Task.CompletedTask);
    }
}
