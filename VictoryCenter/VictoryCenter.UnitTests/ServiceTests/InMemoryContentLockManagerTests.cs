using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services.Concurrency;

namespace VictoryCenter.UnitTests.ServiceTests;

public class InMemoryContentLockManagerTests
{
    private readonly TestTimeProvider _timeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
    private readonly IContentLockManager _manager;
    private readonly ContentLockResourceDto _resource = new()
    {
        ResourceType = ContentLockConstants.EventNewsResourceType,
        ResourceId = 42,
    };

    public InMemoryContentLockManagerTests()
    {
        _manager = new InMemoryContentLockManager(
            _timeProvider,
            Options.Create(new ContentLockOptions { TtlMinutes = 3 }));
    }

    [Fact]
    public void Acquire_WhenResourceIsFree_GrantsOpaqueLock()
    {
        var result = _manager.Acquire(_resource, "admin-a@example.test", "connection-a");

        Assert.True(result.Acquired);
        Assert.NotNull(result.LockId);
        Assert.NotEqual("admin-a@example.test", result.LockId);
        Assert.Equal(_resource.ResourceType, result.ResourceType);
        Assert.Equal(_resource.ResourceId, result.ResourceId);
    }

    [Fact]
    public void GetStatus_WhenResourceIsLocked_ReturnsItsMetadata()
    {
        _manager.Acquire(_resource, "admin-a@example.test", "connection-a");

        var result = _manager.GetStatus(_resource);

        Assert.True(result.IsLocked);
        Assert.Equal("admin-a@example.test", result.OwnerIdentity);
        Assert.NotNull(result.AcquiredAtUtc);
        Assert.NotNull(result.ExpiresAtUtc);
    }

    [Fact]
    public void Acquire_WhenSameAdminUsesAnotherConnection_DeniesSecondSession()
    {
        _manager.Acquire(_resource, "admin@example.test", "connection-a");

        var result = _manager.Acquire(_resource, "admin@example.test", "connection-b");

        Assert.False(result.Acquired);
    }

    [Fact]
    public void RenewAndRelease_RequireSameOwnerAndConnection()
    {
        var acquired = _manager.Acquire(_resource, "admin@example.test", "connection-a");

        var foreignRenew = _manager.Renew(acquired.LockId!, _resource, "admin@example.test", "connection-b");
        var foreignRelease = _manager.Release(acquired.LockId!, _resource, "other@example.test", "connection-a");
        var renewed = _manager.Renew(acquired.LockId!, _resource, "admin@example.test", "connection-a");
        var released = _manager.Release(acquired.LockId!, _resource, "admin@example.test", "connection-a");

        Assert.False(foreignRenew.Acquired);
        Assert.False(foreignRelease.Accepted);
        Assert.True(renewed.Acquired);
        Assert.True(released.Accepted);
        Assert.NotNull(released.ReleasedLock);
    }

    [Fact]
    public void Expiry_IsReleasedByExpirationScanAndAllowsNewSession()
    {
        _manager.Acquire(_resource, "admin@example.test", "connection-a");
        _timeProvider.Advance(TimeSpan.FromMinutes(3));

        Assert.True(_manager.GetStatus(_resource).IsLocked);
        var expired = Assert.Single(_manager.ReleaseExpired());
        var next = _manager.Acquire(_resource, "other@example.test", "connection-b");

        Assert.Equal(_resource.ResourceType, expired.ResourceType);
        Assert.Equal(_resource.ResourceId, expired.ResourceId);
        Assert.False(_manager.GetStatus(_resource).IsLocked);
        Assert.True(next.Acquired);
    }

    [Fact]
    public void ReleaseAllForConnection_ReleasesOnlyThatConnection()
    {
        var first = new ContentLockResourceDto
        {
            ResourceType = ContentLockConstants.EventNewsResourceType,
            ResourceId = 1,
        };
        var second = new ContentLockResourceDto
        {
            ResourceType = ContentLockConstants.EventNewsResourceType,
            ResourceId = 2,
        };
        _manager.Acquire(first, "admin@example.test", "connection-a");
        _manager.Acquire(second, "admin@example.test", "connection-b");

        var released = _manager.ReleaseAllForConnection("connection-a");

        Assert.Single(released);
        Assert.False(_manager.GetStatus(first).IsLocked);
        Assert.True(_manager.GetStatus(second).IsLocked);
    }

    [Fact]
    public void TryBeginUpdate_PreventsExpiryUntilCompletion()
    {
        var acquired = _manager.Acquire(_resource, "admin@example.test", "connection-a");

        _timeProvider.Advance(TimeSpan.FromMinutes(2).Add(TimeSpan.FromSeconds(59)));
        var begun = _manager.TryBeginUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a");
        _timeProvider.Advance(TimeSpan.FromMinutes(2));

        Assert.True(begun);
        Assert.Empty(_manager.ReleaseExpired());
        var status = _manager.GetStatus(_resource);
        Assert.True(status.IsLocked);
        Assert.True(status.IsUpdateInProgress);
        Assert.Null(status.ExpiresAtUtc);
        Assert.False(_manager.Acquire(_resource, "other@example.test", "connection-b").Acquired);
    }

    [Fact]
    public void EndUpdate_ReturnsLockToNormalEditLease()
    {
        var acquired = _manager.Acquire(_resource, "admin@example.test", "connection-a");
        Assert.True(_manager.TryBeginUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a"));

        _manager.EndUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a");
        _timeProvider.Advance(TimeSpan.FromMinutes(3));

        Assert.False(_manager.GetStatus(_resource).IsLocked);
    }

    [Fact]
    public void ReleaseAllForConnection_DefersReleaseUntilUpdateCompletes()
    {
        var acquired = _manager.Acquire(_resource, "admin@example.test", "connection-a");
        Assert.True(_manager.TryBeginUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a"));

        var released = _manager.ReleaseAllForConnection("connection-a");
        var releasedAfterUpdate = _manager.EndUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a");

        Assert.Empty(released);
        Assert.NotNull(releasedAfterUpdate);
        Assert.False(_manager.GetStatus(_resource).IsLocked);
    }

    [Fact]
    public void Release_DuringUpdate_DefersReleaseUntilUpdateCompletes()
    {
        var acquired = _manager.Acquire(_resource, "admin@example.test", "connection-a");
        Assert.True(_manager.TryBeginUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a"));

        var releaseResult = _manager.Release(acquired.LockId!, _resource, "admin@example.test", "connection-a");
        var releasedAfterUpdate = _manager.EndUpdate(acquired.LockId!, _resource, "admin@example.test", "connection-a");

        Assert.True(releaseResult.Accepted);
        Assert.Null(releaseResult.ReleasedLock);
        Assert.NotNull(releasedAfterUpdate);
        Assert.False(_manager.GetStatus(_resource).IsLocked);
    }

    private sealed class TestTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public TestTimeProvider(DateTimeOffset utcNow) => _utcNow = utcNow;

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public void Advance(TimeSpan duration) => _utcNow = _utcNow.Add(duration);
    }
}
