using System.Transactions;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Commands;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.MainPage;

public class ReorderMetricsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMetricRepository> _metricRepositoryMock;
    private readonly IValidator<ReorderMetricsCommand> _validator;

    public ReorderMetricsHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _metricRepositoryMock = new Mock<IMetricRepository>();
        _validator = new ReorderMetricsCommandValidator();

        _repositoryWrapperMock
            .SetupGet(x => x.MetricRepository)
            .Returns(_metricRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(x => x.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
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
        var metrics = orderedIds
            .Reverse()
            .Select((id, index) => new Metric
            {
                Id = id,
                StatisticId = 1L,
                Priority = index,
                IsHidden = false
            })
            .ToList();

        SetupMetricRepository(metrics);

        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        Assert.Equal(orderedIds, metrics.OrderBy(m => m.Priority).Select(m => m.Id));
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

        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReorderWithHiddenMetric_ShouldReorderIncludingHiddenMetric()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new() { Id = 1, StatisticId = 1L, Priority = 0, IsHidden = false },
            new() { Id = 2, StatisticId = 1L, Priority = 1, IsHidden = true },
            new() { Id = 3, StatisticId = 1L, Priority = 2, IsHidden = false },
            new() { Id = 4, StatisticId = 1L, Priority = 3, IsHidden = false }
        };
        SetupMetricRepository(metrics);

        var reorderDto = new ReorderMetricsDto { OrderedIds = [4, 2, 1, 3], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);
        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal([4L, 2L, 1L, 3L], metrics.OrderBy(m => m.Priority).Select(m => m.Id));
        Assert.Equal([0L, 1L, 2L, 3L], metrics.OrderBy(m => m.Priority).Select(m => m.Priority));
    }

    [Fact]
    public async Task Handle_ReorderWithPriorityGaps_ShouldRenumberMetricsContiguously()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new() { Id = 1, StatisticId = 1L, Priority = 0, IsHidden = false },
            new() { Id = 2, StatisticId = 1L, Priority = 2, IsHidden = false },
            new() { Id = 3, StatisticId = 1L, Priority = 3, IsHidden = false },
            new() { Id = 4, StatisticId = 1L, Priority = 4, IsHidden = false }
        };
        SetupMetricRepository(metrics);

        var reorderDto = new ReorderMetricsDto { OrderedIds = [4, 1, 2, 3], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);
        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal([4L, 1L, 2L, 3L], metrics.OrderBy(m => m.Priority).Select(m => m.Id));
        Assert.Equal([0L, 1L, 2L, 3L], metrics.OrderBy(m => m.Priority).Select(m => m.Priority));
    }

    [Fact]
    public async Task Handle_HiddenMetricCanChangePositionNormally()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new() { Id = 1, StatisticId = 1L, Priority = 0, IsHidden = false },
            new() { Id = 2, StatisticId = 1L, Priority = 1, IsHidden = true },
            new() { Id = 3, StatisticId = 1L, Priority = 3, IsHidden = false },
            new() { Id = 4, StatisticId = 1L, Priority = 4, IsHidden = false }
        };
        SetupMetricRepository(metrics);

        var reorderDto = new ReorderMetricsDto { OrderedIds = [4, 2, 1, 3], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);
        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal([4L, 2L, 1L, 3L], metrics.OrderBy(m => m.Priority).Select(m => m.Id));
        Assert.Equal([0L, 1L, 2L, 3L], metrics.OrderBy(m => m.Priority).Select(m => m.Priority));
    }

    [Fact]
    public async Task Handle_MissingMetricId_ShouldReturnFailure()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new() { Id = 1, StatisticId = 1L, Priority = 0, IsHidden = false },
            new() { Id = 2, StatisticId = 1L, Priority = 1, IsHidden = false },
            new() { Id = 3, StatisticId = 1L, Priority = 2, IsHidden = false }
        };
        SetupMetricRepository(metrics);

        var reorderDto = new ReorderMetricsDto { OrderedIds = [2, 1], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);
        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ReorderConstants.ErrorWithReordering(ReorderConstants.NotAllEntitiesFoundForReorder(foundCount: 2, expectedCount: 3)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ForeignMetricId_ShouldReturnFailure()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new() { Id = 1, StatisticId = 1L, Priority = 0, IsHidden = false },
            new() { Id = 2, StatisticId = 1L, Priority = 1, IsHidden = false },
            new() { Id = 3, StatisticId = 1L, Priority = 2, IsHidden = false }
        };
        SetupMetricRepository(metrics);

        var reorderDto = new ReorderMetricsDto { OrderedIds = [2, 1, 999], StatisticId = 1L };
        var command = new ReorderMetricsCommand(reorderDto);
        var handler = new ReorderMetricsHandler(_validator, _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ReorderConstants.ErrorWithReordering(ReorderConstants.NotAllEntitiesFoundForReorder(foundCount: 2, expectedCount: 3)),
            result.Errors[0].Message);
    }

    private void SetupMetricRepository(List<Metric> metrics)
    {
        _metricRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<Metric>>()))
            .ReturnsAsync((QueryOptions<Metric>? options) =>
            {
                IEnumerable<Metric> query = metrics;

                if (options?.Filter is not null)
                {
                    query = query.Where(options.Filter.Compile());
                }

                return query;
            });

        _metricRepositoryMock
            .Setup(x => x.Update(It.IsAny<Metric>()))
            .Returns((Metric metric) => null!);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
    }
}
