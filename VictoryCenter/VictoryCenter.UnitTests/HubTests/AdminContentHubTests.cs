using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.WebAPI.Hubs;

namespace VictoryCenter.UnitTests.HubTests;

public class AdminContentHubTests
{
    private const string ConnectionId = "connection-id";
    private const string OwnerIdentity = "admin@example.test";
    private static readonly ContentLockResourceDto Resource = new()
    {
        ResourceType = ContentLockConstants.EventNewsResourceType,
        ResourceId = 1,
    };

    [Fact]
    public async Task AcquireLock_WhenAcquired_AddsGroupAndNotifiesResourceEditors()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out var groupClient, out _);
        var groups = CreateGroups();
        var acquisition = new ContentLockAcquisitionDto
        {
            Acquired = true,
            LockId = "lock-id",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(3),
            ResourceType = Resource.ResourceType,
            ResourceId = Resource.ResourceId,
        };
        lockManager.Setup(item => item.Acquire(Resource, OwnerIdentity, ConnectionId)).Returns(acquisition);
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.AcquireLock(Resource);

        Assert.Same(acquisition, result);
        groups.Verify(item => item.AddToGroupAsync(ConnectionId, Resource.Key, It.IsAny<CancellationToken>()), Times.Once);
        groupClient.Verify(
            item => item.SendCoreAsync(
                ContentLockEvents.Acquired,
                It.Is<object?[]>(arguments => HasLifecycle(arguments, acquisition.LockId!, OwnerIdentity)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AcquireLock_WhenDenied_NotifiesCallerWithCurrentStatus()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out _, out var callerClient);
        var groups = CreateGroups();
        var status = new ContentLockStatusDto { IsLocked = true, ResourceType = Resource.ResourceType, ResourceId = Resource.ResourceId };
        lockManager.Setup(item => item.Acquire(Resource, OwnerIdentity, ConnectionId)).Returns(new ContentLockAcquisitionDto { Acquired = false });
        lockManager.Setup(item => item.GetStatus(Resource)).Returns(status);
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.AcquireLock(Resource);

        Assert.False(result.Acquired);
        callerClient.Verify(
            item => item.SendCoreAsync(
                ContentLockEvents.Denied,
                It.Is<object?[]>(arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], status)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AcquireLock_WhenNotificationFails_ReturnsAcquiredLock()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out var groupClient, out _);
        var groups = CreateGroups();
        var acquisition = new ContentLockAcquisitionDto
        {
            Acquired = true,
            LockId = "lock-id",
            ResourceType = Resource.ResourceType,
            ResourceId = Resource.ResourceId,
        };
        groupClient
            .Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SignalR is unavailable."));
        lockManager.Setup(item => item.Acquire(Resource, OwnerIdentity, ConnectionId)).Returns(acquisition);
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.AcquireLock(Resource);

        Assert.Same(acquisition, result);
    }

    [Fact]
    public async Task AcquireLock_WhenDeniedNotificationFails_ReturnsDeniedResult()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out _, out var callerClient);
        var groups = CreateGroups();
        callerClient
            .Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SignalR is unavailable."));
        lockManager.Setup(item => item.Acquire(Resource, OwnerIdentity, ConnectionId)).Returns(new ContentLockAcquisitionDto { Acquired = false });
        lockManager.Setup(item => item.GetStatus(Resource)).Returns(new ContentLockStatusDto { IsLocked = true });
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.AcquireLock(Resource);

        Assert.False(result.Acquired);
    }

    [Fact]
    public async Task RenewLock_DelegatesToLockManagerForCurrentConnection()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out _, out _);
        var groups = CreateGroups();
        var expected = new ContentLockAcquisitionDto { Acquired = true, LockId = "renewed-lock" };
        lockManager.Setup(item => item.Renew("lock-id", Resource, OwnerIdentity, ConnectionId)).Returns(expected);
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.RenewLock(Resource, "lock-id");

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task ReleaseLock_WhenReleased_NotifiesResourceEditors()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out var groupClient, out _);
        var groups = CreateGroups();
        var released = new ContentLockLifecycleDto
        {
            ResourceType = Resource.ResourceType,
            ResourceId = Resource.ResourceId,
            LockId = "lock-id",
            OwnerIdentity = OwnerIdentity,
        };
        lockManager
            .Setup(item => item.Release("lock-id", Resource, OwnerIdentity, ConnectionId))
            .Returns(new ContentLockReleaseResult { Accepted = true, ReleasedLock = released });
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var releasedByHub = await hub.ReleaseLock(Resource, "lock-id");

        Assert.True(releasedByHub);
        groupClient.Verify(
            item => item.SendCoreAsync(
                ContentLockEvents.Released,
                It.Is<object?[]>(arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], released)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReleaseLock_WhenNotificationFails_ReturnsSuccessfulRelease()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out var groupClient, out _);
        var groups = CreateGroups();
        var released = new ContentLockLifecycleDto
        {
            ResourceType = Resource.ResourceType,
            ResourceId = Resource.ResourceId,
            LockId = "lock-id",
            OwnerIdentity = OwnerIdentity,
        };
        groupClient
            .Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SignalR is unavailable."));
        lockManager
            .Setup(item => item.Release("lock-id", Resource, OwnerIdentity, ConnectionId))
            .Returns(new ContentLockReleaseResult { Accepted = true, ReleasedLock = released });
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.ReleaseLock(Resource, "lock-id");

        Assert.True(result);
    }

    [Fact]
    public async Task GetLockStatus_AddsCallerToResourceGroup()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out _, out _);
        var groups = CreateGroups();
        var status = new ContentLockStatusDto { IsLocked = false, ResourceType = Resource.ResourceType, ResourceId = Resource.ResourceId };
        lockManager.Setup(item => item.GetStatus(Resource)).Returns(status);
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var result = await hub.GetLockStatus(Resource);

        Assert.Same(status, result);
        groups.Verify(item => item.AddToGroupAsync(ConnectionId, Resource.Key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnDisconnectedAsync_ReleasesAndNotifiesAllConnectionLocks()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = SupportedPolicy();
        var clients = CreateClients(out var groupClient, out _);
        var groups = CreateGroups();
        var released = new ContentLockLifecycleDto
        {
            ResourceType = Resource.ResourceType,
            ResourceId = Resource.ResourceId,
            LockId = "lock-id",
            OwnerIdentity = OwnerIdentity,
        };
        lockManager.Setup(item => item.ReleaseAllForConnection(ConnectionId)).Returns([released]);
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        await hub.OnDisconnectedAsync(null);

        groupClient.Verify(
            item => item.SendCoreAsync(
                ContentLockEvents.Released,
                It.Is<object?[]>(arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], released)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task OnDisconnectedAsync_WhenOneNotificationFails_ContinuesWithRemainingLocks()
    {
        var secondResource = new ContentLockResourceDto
        {
            ResourceType = Resource.ResourceType,
            ResourceId = 2,
        };
        var firstReleased = new ContentLockLifecycleDto
        {
            ResourceType = Resource.ResourceType,
            ResourceId = Resource.ResourceId,
            LockId = "first-lock-id",
            OwnerIdentity = OwnerIdentity,
        };
        var secondReleased = new ContentLockLifecycleDto
        {
            ResourceType = secondResource.ResourceType,
            ResourceId = secondResource.ResourceId,
            LockId = "second-lock-id",
            OwnerIdentity = OwnerIdentity,
        };
        var firstGroupClient = ClientProxy();
        firstGroupClient
            .Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SignalR is unavailable."));
        var secondGroupClient = ClientProxy();
        var clients = new Mock<IHubCallerClients>();
        clients.Setup(item => item.Group(Resource.Key)).Returns(firstGroupClient.Object);
        clients.Setup(item => item.Group(secondResource.Key)).Returns(secondGroupClient.Object);
        var lockManager = new Mock<IContentLockManager>();
        lockManager.Setup(item => item.ReleaseAllForConnection(ConnectionId)).Returns([firstReleased, secondReleased]);
        var hub = CreateHub(lockManager.Object, SupportedPolicy().Object, clients.Object, CreateGroups().Object);

        await hub.OnDisconnectedAsync(null);

        secondGroupClient.Verify(
            item => item.SendCoreAsync(
                ContentLockEvents.Released,
                It.Is<object?[]>(arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], secondReleased)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AcquireLock_WhenResourceIsUnsupported_ThrowsHubException()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = new Mock<IContentLockResourcePolicy>();
        policy.Setup(item => item.IsSupported(Resource)).Returns(false);
        var clients = CreateClients(out _, out _);
        var groups = CreateGroups();
        var hub = CreateHub(lockManager.Object, policy.Object, clients.Object, groups.Object);

        var exception = await Assert.ThrowsAsync<HubException>(() => hub.AcquireLock(Resource));

        Assert.Equal(ContentLockConstants.UnsupportedResourceMessage, exception.Message);
        lockManager.Verify(item => item.Acquire(It.IsAny<ContentLockResourceDto>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    private static AdminContentHub CreateHub(
        IContentLockManager lockManager,
        IContentLockResourcePolicy resourcePolicy,
        IHubCallerClients clients,
        IGroupManager groups)
    {
        var context = new Mock<HubCallerContext>();
        context.SetupGet(item => item.ConnectionId).Returns(ConnectionId);
        context.SetupGet(item => item.User).Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, OwnerIdentity)])));

        return new AdminContentHub(lockManager, resourcePolicy, Mock.Of<ILogger<AdminContentHub>>())
        {
            Context = context.Object,
            Clients = clients,
            Groups = groups,
        };
    }

    private static Mock<IContentLockResourcePolicy> SupportedPolicy()
    {
        var policy = new Mock<IContentLockResourcePolicy>();
        policy.Setup(item => item.IsSupported(Resource)).Returns(true);
        return policy;
    }

    private static Mock<IHubCallerClients> CreateClients(out Mock<IClientProxy> groupClient, out Mock<ISingleClientProxy> callerClient)
    {
        groupClient = ClientProxy();
        callerClient = SingleClientProxy();
        var clients = new Mock<IHubCallerClients>();
        clients.Setup(item => item.Group(Resource.Key)).Returns(groupClient.Object);
        clients.SetupGet(item => item.Caller).Returns(callerClient.Object);
        return clients;
    }

    private static Mock<IGroupManager> CreateGroups()
    {
        var groups = new Mock<IGroupManager>();
        groups.Setup(item => item.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return groups;
    }

    private static Mock<IClientProxy> ClientProxy()
    {
        var client = new Mock<IClientProxy>();
        client.Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return client;
    }

    private static Mock<ISingleClientProxy> SingleClientProxy()
    {
        var client = new Mock<ISingleClientProxy>();
        client.Setup(item => item.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return client;
    }

    private static bool HasLifecycle(object?[] arguments, string lockId, string ownerIdentity)
    {
        return arguments.Length == 1
               && arguments[0] is ContentLockLifecycleDto lifecycle
               && lifecycle.LockId == lockId
               && lifecycle.OwnerIdentity == ownerIdentity;
    }
}
