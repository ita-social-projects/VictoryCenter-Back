using Moq;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.BLL.Services.FundsMetricSync;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Notifications.ReportFunds;

public class SyncRaisedFundsMetricHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMetricRepository> _metricRepositoryMock = new();
    private readonly Mock<IRaisedFundsMetricSyncService> _syncServiceMock = new();

    public SyncRaisedFundsMetricHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.MetricRepository)
            .Returns(_metricRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnImmediately_WhenSkipRaisedMetricSyncIsTrue()
    {
        var notification = new ReportFundsChangedNotification { SkipRaisedMetricSync = true };
        var handler = CreateHandler();

        await handler.Handle(notification, CancellationToken.None);

        _metricRepositoryMock.Verify(
            repo => repo.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>()),
            Times.Never);

        _syncServiceMock.Verify(
            service => service.ApplySyncAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFindMetric_ApplySync_AndSaveChanges_WhenSkipRaisedMetricSyncIsFalseAndChangesOccur()
    {
        // Arrange
        var metric = new Metric
        {
            Id = 10,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 100
        };

        SetupRaisedMetricInDb(metric);

        _syncServiceMock
            .Setup(service => service.ApplySyncAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var notification = new ReportFundsChangedNotification { SkipRaisedMetricSync = false };
        var handler = CreateHandler();

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        _metricRepositoryMock.Verify(
            repo => repo.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>()),
            Times.Once);

        _syncServiceMock.Verify(
            service => service.ApplySyncAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotSaveChanges_WhenSyncServiceReportsNoChanges()
    {
        var metric = new Metric
        {
            Id = 10,
            Type = MetricType.Raised,
            IsAutoSynced = true,
            Value = 5000
        };

        SetupRaisedMetricInDb(metric);

        _syncServiceMock
            .Setup(service => service.ApplySyncAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var notification = new ReportFundsChangedNotification { SkipRaisedMetricSync = false };
        var handler = CreateHandler();

        await handler.Handle(notification, CancellationToken.None);

        _syncServiceMock.Verify(
            service => service.ApplySyncAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenRaisedMetricNotFoundInDb()
    {
        SetupRaisedMetricInDb(null);

        var notification = new ReportFundsChangedNotification { SkipRaisedMetricSync = false };
        var handler = CreateHandler();

        await handler.Handle(notification, CancellationToken.None);

        _syncServiceMock.Verify(
            service => service.ApplySyncAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(),
            Times.Never);
    }

    private SyncRaisedFundsMetricHandler CreateHandler() =>
        new(_repositoryWrapperMock.Object, _syncServiceMock.Object);

    private void SetupRaisedMetricInDb(Metric? metric)
    {
        _metricRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Metric>?>()))
            .ReturnsAsync((QueryOptions<Metric>? _) => metric);
    }
}
