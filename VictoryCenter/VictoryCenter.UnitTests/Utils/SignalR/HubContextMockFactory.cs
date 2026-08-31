using Microsoft.AspNetCore.SignalR;
using Moq;

namespace VictoryCenter.UnitTests.Utils.SignalR;

public static class HubContextMockFactory
{
    public static Mock<IHubContext<THub>> Create<THub>()
        where THub : Hub
    {
        var clientProxy = new Mock<IClientProxy>();
        clientProxy
            .Setup(proxy => proxy.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hubClients = new Mock<IHubClients>();
        hubClients.SetupGet(clients => clients.All).Returns(clientProxy.Object);

        var hubContext = new Mock<IHubContext<THub>>();
        hubContext.SetupGet(context => context.Clients).Returns(hubClients.Object);
        return hubContext;
    }
}
