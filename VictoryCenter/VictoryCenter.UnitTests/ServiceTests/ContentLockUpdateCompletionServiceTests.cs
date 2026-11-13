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

    [Fact]
    public async Task CompleteAsync_PublishesWithoutHttpRequestCancellation()
    {
        var notifier = new Mock<IContentLockNotifier>();
        CancellationToken entityChangedCancellationToken = new(true);
        notifier
            .Setup(item => item.NotifyEntityChangedAsync(It.IsAny<ContentEntityChangedDto>(), It.IsAny<CancellationToken>()))
            .Callback<ContentEntityChangedDto, CancellationToken>((_, cancellationToken) => entityChangedCancellationToken = cancellationToken)
            .Returns(Task.CompletedTask);
        var service = CreateService(notifier.Object);

        await service.CompleteAsync(Resource);

        Assert.Equal(CancellationToken.None, entityChangedCancellationToken);
        notifier.Verify(
            item => item.NotifyEntityChangedAsync(
                It.Is<ContentEntityChangedDto>(entityChanged => entityChanged.ResourceType == Resource.ResourceType
                                                           && entityChanged.ResourceId == Resource.ResourceId),
                CancellationToken.None),
            Times.Once);
        notifier.Verify(
            item => item.NotifyLockReleasedAsync(It.IsAny<ContentLockLifecycleDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CompleteAsync_WhenNotificationFails_DoesNotThrowAfterPersistence()
    {
        var notifier = new Mock<IContentLockNotifier>();
        notifier
            .Setup(item => item.NotifyEntityChangedAsync(It.IsAny<ContentEntityChangedDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SignalR is unavailable."));
        var service = CreateService(notifier.Object);

        var exception = await Record.ExceptionAsync(() => service.CompleteAsync(Resource));

        Assert.Null(exception);
        notifier.Verify(
            item => item.NotifyLockReleasedAsync(It.IsAny<ContentLockLifecycleDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static ContentLockUpdateCompletionService CreateService(IContentLockNotifier notifier)
    {
        return new ContentLockUpdateCompletionService(
            notifier,
            Mock.Of<ILogger<ContentLockUpdateCompletionService>>());
    }
}
