using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ToggleMetricVisibility;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Interfaces.MainPage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class ToggleMetricVisibilityHandlerTests
{
    private readonly Mock<IMetricVisibilityService> _metricVisibilityServiceMock = new();

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenServiceSucceeds()
    {
        // Arrange
        var command = new ToggleMetricVisibilityCommand(1, new UpdateMetricVisibilityDto { IsHidden = true });

        _metricVisibilityServiceMock
            .Setup(x => x.ToggleMetricVisibilityAsync(command.MetricId, command.Dto.IsHidden))
            .Returns(Task.CompletedTask);

        var handler = new ToggleMetricVisibilityHandler(_metricVisibilityServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _metricVisibilityServiceMock.Verify(x => x.ToggleMetricVisibilityAsync(command.MetricId, command.Dto.IsHidden), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenServiceThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new ToggleMetricVisibilityCommand(1, new UpdateMetricVisibilityDto { IsHidden = true });
        var exceptionMessage = "Metric not found";

        _metricVisibilityServiceMock
            .Setup(x => x.ToggleMetricVisibilityAsync(It.IsAny<long>(), It.IsAny<bool>()))
            .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

        var handler = new ToggleMetricVisibilityHandler(_metricVisibilityServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        var command = new ToggleMetricVisibilityCommand(1, new UpdateMetricVisibilityDto { IsHidden = true });
        var exceptionMessage = "Cannot hide the last visible metric.";

        _metricVisibilityServiceMock
            .Setup(x => x.ToggleMetricVisibilityAsync(It.IsAny<long>(), It.IsAny<bool>()))
            .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        var handler = new ToggleMetricVisibilityHandler(_metricVisibilityServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenServiceThrowsDbUpdateException()
    {
        // Arrange
        var command = new ToggleMetricVisibilityCommand(1, new UpdateMetricVisibilityDto { IsHidden = true });

        _metricVisibilityServiceMock
            .Setup(x => x.ToggleMetricVisibilityAsync(It.IsAny<long>(), It.IsAny<bool>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new ToggleMetricVisibilityHandler(_metricVisibilityServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Metric)), result.Errors.First().Message);
    }
}