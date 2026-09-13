using System.Linq.Expressions;
using FluentValidation;
using MediatR;
using Moq;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.VideoReviews;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.VideoReviews;

public class ReorderVideoReviewsTests
{
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderVideoReviewsCommand> _validator;

    private readonly ReorderVideoReviewsDto _testValidReorderDto = new()
    {
        OrderedIds = [4, 2, 5, 1, 3]
    };

    public ReorderVideoReviewsTests()
    {
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderVideoReviewsValidator();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReorderVideoReviews()
    {
        SetupReorderService();

        var handler = new ReorderVideoReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderVideoReviewsCommand(_testValidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);

        _mockReorderService.Verify(
            x => x.SwapElementsAsync(
                It.Is<List<long>>(ids => ids.SequenceEqual(_testValidReorderDto.OrderedIds)),
                It.IsAny<Expression<Func<VideoReview, long>>>(),
                It.IsAny<Expression<Func<VideoReview, bool>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyOrderedIds_ShouldReturnError()
    {
        var invalidReorderDto = new ReorderVideoReviewsDto
        {
            OrderedIds = []
        };

        SetupReorderService();
        var handler = new ReorderVideoReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderVideoReviewsCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderVideoReviewsDto.OrderedIds)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DuplicateIdsInOrderedIds_ShouldReturnError()
    {
        var invalidReorderDto = new ReorderVideoReviewsDto
        {
            OrderedIds = [1, 2, 2, 3]
        };

        SetupReorderService();
        var handler = new ReorderVideoReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderVideoReviewsCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(ReorderVideoReviewsDto.OrderedIds)),
            result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_InvalidIdInOrderedIds_ShouldReturnError(long invalidId)
    {
        var invalidReorderDto = new ReorderVideoReviewsDto
        {
            OrderedIds = [1, 2, invalidId]
        };

        SetupReorderService();
        var handler = new ReorderVideoReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderVideoReviewsCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBePositive(
                $"Each {nameof(ReorderVideoReviewsDto.OrderedIds)} element"),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderServiceThrowsReorderException_ShouldReturnReorderError()
    {
        var reorderExceptionMessage = "Reorder operation failed";

        _mockReorderService.Setup(x => x.SwapElementsAsync<VideoReview>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<VideoReview, long>>>(),
                It.IsAny<Expression<Func<VideoReview, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderExceptionMessage));

        var handler = new ReorderVideoReviewsHandler(_validator, _mockReorderService.Object);
        var command = new ReorderVideoReviewsCommand(_testValidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderExceptionMessage), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(x => x.SwapElementsAsync<VideoReview>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<VideoReview, long>>>(),
                It.IsAny<Expression<Func<VideoReview, bool>>>()))
            .Returns(Task.CompletedTask);
    }
}
