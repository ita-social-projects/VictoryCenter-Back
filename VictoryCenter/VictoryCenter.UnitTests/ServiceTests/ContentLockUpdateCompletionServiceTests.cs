using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.BLL.Services.Concurrency;

namespace VictoryCenter.UnitTests.ServiceTests;

public class ContentLockUpdateCompletionServiceTests
{
    private static readonly ContentLockResourceDto Resource = new()
    {
        ResourceType = ContentLockConstants.EventNewsResourceType,
        ResourceId = 1,
    };

    private static readonly ContentLockSessionDto Session = new()
    {
        LockId = "lock-id",
        OwnerIdentity = "admin@example.test",
        ConnectionId = "connection-id",
    };

    [Fact]
    public async Task CompleteAsync_PublishesWithoutHttpRequestCancellation()
    {
        var manager = new Mock<IContentLockManager>();
        var notifier = new Mock<IContentLockNotifier>();
        CancellationToken entityChangedCancellationToken = new(true);
        manager
            .Setup(item => item.Release(Session.LockId, Resource, Session.OwnerIdentity, Session.ConnectionId))
            .Returns(new ContentLockReleaseResult { Accepted = true });
        notifier
            .Setup(item => item.NotifyEntityChangedAsync(It.IsAny<ContentEntityChangedDto>(), It.IsAny<CancellationToken>()))
            .Callback<ContentEntityChangedDto, CancellationToken>((_, cancellationToken) => entityChangedCancellationToken = cancellationToken)
            .Returns(Task.CompletedTask);
        var service = CreateService(manager.Object, notifier.Object);

        await service.CompleteAsync(Resource, Session);

        Assert.Equal(CancellationToken.None, entityChangedCancellationToken);
    }

    [Fact]
    public async Task CompleteAsync_WhenNotificationFails_DoesNotThrowAfterPersistence()
    {
        var manager = new Mock<IContentLockManager>();
        var notifier = new Mock<IContentLockNotifier>();
        manager
            .Setup(item => item.Release(Session.LockId, Resource, Session.OwnerIdentity, Session.ConnectionId))
            .Returns(new ContentLockReleaseResult
            {
                Accepted = true,
                ReleasedLock = new ContentLockLifecycleDto
                {
                    ResourceType = Resource.ResourceType,
                    ResourceId = Resource.ResourceId,
                    LockId = Session.LockId,
                    OwnerIdentity = Session.OwnerIdentity,
                },
            });
        notifier
            .Setup(item => item.NotifyEntityChangedAsync(It.IsAny<ContentEntityChangedDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SignalR is unavailable."));
        var service = CreateService(manager.Object, notifier.Object);

        var exception = await Record.ExceptionAsync(() => service.CompleteAsync(Resource, Session));

        Assert.Null(exception);
        notifier.Verify(
            item => item.NotifyLockReleasedAsync(It.IsAny<ContentLockLifecycleDto>(), CancellationToken.None),
            Times.Once);
    }

    private static ContentLockUpdateCompletionService CreateService(
        IContentLockManager lockManager,
        IContentLockNotifier notifier)
    {
        return new ContentLockUpdateCompletionService(
            lockManager,
            notifier,
            Mock.Of<ILogger<ContentLockUpdateCompletionService>>());
    }
}
