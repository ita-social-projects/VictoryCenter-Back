using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Notifications.ReportFunds;

public class MarkReportFundsChangedHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresSettingsRepository> _settingsRepoMock;
    private readonly TimeProvider _timeProvider;

    public MarkReportFundsChangedHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _settingsRepoMock = new Mock<IReportFundsExpendituresSettingsRepository>();

        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresSettingsRepository).Returns(_settingsRepoMock.Object);
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task Handle_ShouldReturnEarly_WhenGetOrCreateSettingsFails()
    {
        // Arrange
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>>()))
            .ReturnsAsync((global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings?)null);

        _repositoryWrapperMock.Setup(w => w.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new MarkReportFundsChangedHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetHasUnpublishedChanges_WhenFalse()
    {
        // Arrange
        var settings = new global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings { HasUnpublishedChanges = false };
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>>()))
            .ReturnsAsync(settings);

        var handler = new MarkReportFundsChangedHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        Assert.True(settings.HasUnpublishedChanges);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotSaveChanges_WhenHasUnpublishedChangesIsTrue()
    {
        // Arrange
        var settings = new global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings { HasUnpublishedChanges = true };
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>>()))
            .ReturnsAsync(settings);

        var handler = new MarkReportFundsChangedHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        await handler.Handle(new ReportFundsChangedNotification(), CancellationToken.None);

        // Assert
        Assert.True(settings.HasUnpublishedChanges);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
    }
}
