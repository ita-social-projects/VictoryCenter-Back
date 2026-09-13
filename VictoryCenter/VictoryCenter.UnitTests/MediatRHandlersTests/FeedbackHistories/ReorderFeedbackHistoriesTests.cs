using System.Linq.Expressions;
using FluentValidation;
using MediatR;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.FeedbackHistories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class ReorderFeedbackHistoriesTests
{
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderFeedbackHistoriesCommand> _validator;

    private readonly ReorderFeedbackHistoriesDto _testValidReorderDto = new()
    {
        OrderedIds = [4, 2, 5, 1, 3]
    };

    public ReorderFeedbackHistoriesTests()
    {
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderFeedbackHistoriesValidator();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldReorderHistories()
    {
        SetupReorderService();

        var handler = new ReorderFeedbackHistoriesHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackHistoriesCommand(_testValidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);

        _mockReorderService.Verify(
            x => x.SwapElementsAsync(
                It.Is<List<long>>(ids => ids.SequenceEqual(_testValidReorderDto.OrderedIds)),
                It.IsAny<Expression<Func<FeedbackHistory, long>>>(),
                It.IsAny<Expression<Func<FeedbackHistory, bool>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyOrderedIds_ShouldReturnError()
    {
        var invalidReorderDto = new ReorderFeedbackHistoriesDto
        {
            OrderedIds = []
        };

        SetupReorderService();
        var handler = new ReorderFeedbackHistoriesHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackHistoriesCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderFeedbackHistoriesDto.OrderedIds)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DuplicateIdsInOrderedIds_ShouldReturnError()
    {
        var invalidReorderDto = new ReorderFeedbackHistoriesDto
        {
            OrderedIds = [1, 2, 2, 3]
        };

        SetupReorderService();
        var handler = new ReorderFeedbackHistoriesHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackHistoriesCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(ReorderFeedbackHistoriesDto.OrderedIds)),
            result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_InvalidIdInOrderedIds_ShouldReturnError(long invalidId)
    {
        var invalidReorderDto = new ReorderFeedbackHistoriesDto
        {
            OrderedIds = [1, 2, invalidId]
        };

        SetupReorderService();
        var handler = new ReorderFeedbackHistoriesHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackHistoriesCommand(invalidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBePositive(
                $"Each {nameof(ReorderFeedbackHistoriesDto.OrderedIds)} element"),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderServiceThrowsReorderException_ShouldReturnReorderError()
    {
        var reorderExceptionMessage = "Reorder operation failed";

        _mockReorderService.Setup(x => x.SwapElementsAsync<FeedbackHistory>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<FeedbackHistory, long>>>(),
                It.IsAny<Expression<Func<FeedbackHistory, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderExceptionMessage));

        var handler = new ReorderFeedbackHistoriesHandler(_validator, _mockReorderService.Object);
        var command = new ReorderFeedbackHistoriesCommand(_testValidReorderDto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderExceptionMessage), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(x => x.SwapElementsAsync<FeedbackHistory>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<FeedbackHistory, long>>>(),
                It.IsAny<Expression<Func<FeedbackHistory, bool>>>()))
            .Returns(Task.CompletedTask);
    }
}
