using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.MainPage.Commands;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class ReorderMetricsHandlerTests
{
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderMetricsCommand> _validator;

    public ReorderMetricsHandlerTests()
    {
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderMetricsCommandValidator();
    }

    [Theory]
    [InlineData(2L, 1L)]
    [InlineData(3L, 2L, 1L)]
    public async Task Handle_ValidDto_ShouldReturnOk(params long[] orderedIds)
    {
        // Arrange
        var reorderDto = new ReorderMetricsDto
        {
            OrderedIds = [.. orderedIds],
            StatisticId = 1L
        };
        var command = new ReorderMetricsCommand(reorderDto);
        SetupReorderService();

        var handler = new ReorderMetricsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        _mockReorderService.Verify(
            service => service.SwapElementsAsync(
                It.Is<List<long>>(ids => ids.SequenceEqual(orderedIds)),
                It.IsAny<Expression<Func<Metric, long>>>(),
                It.IsAny<Expression<Func<Metric, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidDto_ShouldReturnValidationFailure()
    {
        // Arrange
        var reorderDto = new ReorderMetricsDto
        {
            OrderedIds = [],
            StatisticId = 0L
        };
        var command = new ReorderMetricsCommand(reorderDto);

        var handler = new ReorderMetricsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_DbUpdateExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var reorderDto = new ReorderMetricsDto { OrderedIds = [2, 1], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);

        _mockReorderService.Setup(service => service.SwapElementsAsync<Metric>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<Metric, long>>>(),
            It.IsAny<Expression<Func<Metric, bool>>>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new ReorderMetricsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Metric)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var reorderDto = new ReorderMetricsDto { OrderedIds = [2, 1], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);
        var reorderErrorMessage = "Test metric reorder error";

        _mockReorderService.Setup(service => service.SwapElementsAsync<Metric>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<Metric, long>>>(),
            It.IsAny<Expression<Func<Metric, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderErrorMessage));

        var handler = new ReorderMetricsHandler(_validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderErrorMessage), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService
            .Setup(service => service.SwapElementsAsync<Metric>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<Metric, long>>>(),
                It.IsAny<Expression<Func<Metric, bool>>>()))
            .Returns(Task.CompletedTask);
    }
}