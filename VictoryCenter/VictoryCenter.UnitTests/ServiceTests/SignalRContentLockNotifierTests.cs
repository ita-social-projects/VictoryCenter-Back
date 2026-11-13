using Microsoft.AspNetCore.SignalR;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.WebAPI.Hubs;
using VictoryCenter.WebAPI.Services.Concurrency;

namespace VictoryCenter.UnitTests.ServiceTests;

public class SignalRContentLockNotifierTests
{
    private static readonly ContentLockLifecycleDto Lifecycle = new()
    {
        ResourceType = ContentLockConstants.EventNewsResourceType,
        ResourceId = 1,
        LockId = "lock-id",
        OwnerIdentity = "admin@example.test",
    };

    [Theory]
    [InlineData(ContentLockEvents.Released)]
    [InlineData(ContentLockEvents.Expired)]
    public async Task LifecycleNotification_SendsEventToResourceGroup(string eventName)
    {
        var hubContext = CreateHubContext(out var clients, out var client);
        var notifier = new SignalRContentLockNotifier(hubContext.Object);

        if (eventName == ContentLockEvents.Released)
        {
            await notifier.NotifyLockReleasedAsync(Lifecycle);
        }
        else
        {
            await notifier.NotifyLockExpiredAsync(Lifecycle);
        }

        clients.Verify(item => item.Group("EventNews:1"), Times.Once);
        client.Verify(
            item => item.SendCoreAsync(
                eventName,
                It.Is<object?[]>(arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], Lifecycle)),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task NotifyEntityChangedAsync_SendsEntityEventToResourceGroup()
    {
        var hubContext = CreateHubContext(out var clients, out var client);
        var notifier = new SignalRContentLockNotifier(hubContext.Object);
        var entityChanged = new ContentEntityChangedDto
        {
            ResourceType = ContentLockConstants.EventNewsResourceType,
            ResourceId = 1,
        };

        await notifier.NotifyEntityChangedAsync(entityChanged);

        clients.Verify(item => item.Group("EventNews:1"), Times.Once);
        client.Verify(
            item => item.SendCoreAsync(
                ContentLockEvents.EntityChanged,
                It.Is<object?[]>(arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], entityChanged)),
                CancellationToken.None),
            Times.Once);
    }

    private static Mock<IHubContext<AdminContentHub>> CreateHubContext(
        out Mock<IHubClients> clients,
        out Mock<IClientProxy> client)
    {
        client = new Mock<IClientProxy>();
        client.Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        clients = new Mock<IHubClients>();
        clients.Setup(item => item.Group("EventNews:1")).Returns(client.Object);
        var hubContext = new Mock<IHubContext<AdminContentHub>>();
        hubContext.SetupGet(item => item.Clients).Returns(clients.Object);
        return hubContext;
    }
}
