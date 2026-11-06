using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.BLL.Options;

namespace VictoryCenter.BLL.Services.Concurrency;

public sealed class InMemoryContentLockManager : IContentLockManager
{
    private readonly ConcurrentDictionary<string, ContentLock> _locksByResource = new(StringComparer.Ordinal);
    private readonly TimeProvider _timeProvider;
    private readonly ContentLockOptions _options;

    public InMemoryContentLockManager(TimeProvider timeProvider, IOptions<ContentLockOptions> options)
    {
        _timeProvider = timeProvider;
        _options = options.Value;
    }

    public ContentLockAcquisitionDto Acquire(ContentLockResourceDto resource, string ownerIdentity, string connectionId)
    {
        var now = _timeProvider.GetUtcNow();
        var contentLock = new ContentLock(Guid.NewGuid().ToString("N"), resource, ownerIdentity, connectionId, now, now.Add(_options.Ttl));
        if (_locksByResource.TryAdd(resource.Key, contentLock))
        {
            return ToAcquisition(contentLock, true);
        }

        return Denied(resource);
    }

    public ContentLockAcquisitionDto Renew(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId)
    {
        var now = _timeProvider.GetUtcNow();
        if (!_locksByResource.TryGetValue(resource.Key, out var contentLock)
            || IsExpired(contentLock, now)
            || !Matches(contentLock, lockId, ownerIdentity, connectionId))
        {
            return Denied(resource);
        }

        var renewed = contentLock with { ExpiresAtUtc = now.Add(_options.Ttl) };
        if (_locksByResource.TryUpdate(resource.Key, renewed, contentLock))
        {
            return ToAcquisition(renewed, true);
        }

        return Denied(resource);
    }

    public ContentLockReleaseResult Release(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId)
    {
        var now = _timeProvider.GetUtcNow();
        while (_locksByResource.TryGetValue(resource.Key, out var contentLock))
        {
            if (IsExpired(contentLock, now)
                || !Matches(contentLock, lockId, ownerIdentity, connectionId))
            {
                return DeniedRelease();
            }

            if (contentLock.IsUpdateInProgress)
            {
                if (contentLock.ReleaseAfterUpdate)
                {
                    return AcceptedRelease();
                }

                var releaseAfterUpdate = contentLock with { ReleaseAfterUpdate = true };
                if (_locksByResource.TryUpdate(resource.Key, releaseAfterUpdate, contentLock))
                {
                    return AcceptedRelease();
                }

                continue;
            }

            if (_locksByResource.TryRemove(new KeyValuePair<string, ContentLock>(resource.Key, contentLock)))
            {
                return AcceptedRelease(ToLifecycle(contentLock));
            }
        }

        return DeniedRelease();
    }

    public bool TryBeginUpdate(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId)
    {
        var now = _timeProvider.GetUtcNow();
        if (!_locksByResource.TryGetValue(resource.Key, out var contentLock)
            || IsExpired(contentLock, now)
            || contentLock.IsUpdateInProgress
            || !Matches(contentLock, lockId, ownerIdentity, connectionId))
        {
            return false;
        }

        var updateLock = contentLock with
        {
            IsUpdateInProgress = true,
            ExpiresAtUtc = now.Add(_options.Ttl)
        };
        return _locksByResource.TryUpdate(resource.Key, updateLock, contentLock);
    }

    public ContentLockLifecycleDto? EndUpdate(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId)
    {
        var now = _timeProvider.GetUtcNow();
        while (_locksByResource.TryGetValue(resource.Key, out var contentLock))
        {
            if (!contentLock.IsUpdateInProgress
                || !Matches(contentLock, lockId, ownerIdentity, connectionId))
            {
                return null;
            }

            if (contentLock.ReleaseAfterUpdate)
            {
                if (_locksByResource.TryRemove(new KeyValuePair<string, ContentLock>(resource.Key, contentLock)))
                {
                    return ToLifecycle(contentLock);
                }

                continue;
            }

            var editingLock = contentLock with
            {
                IsUpdateInProgress = false,
                ExpiresAtUtc = now.Add(_options.Ttl)
            };
            if (_locksByResource.TryUpdate(resource.Key, editingLock, contentLock))
            {
                return null;
            }
        }

        return null;
    }

    public ContentLockStatusDto GetStatus(ContentLockResourceDto resource)
    {
        if (_locksByResource.TryGetValue(resource.Key, out var contentLock))
        {
            return new ContentLockStatusDto
            {
                IsLocked = true,
                IsUpdateInProgress = contentLock.IsUpdateInProgress,
                OwnerIdentity = contentLock.OwnerIdentity,
                AcquiredAtUtc = contentLock.AcquiredAtUtc,
                ExpiresAtUtc = contentLock.IsUpdateInProgress ? null : contentLock.ExpiresAtUtc,
                ResourceType = resource.ResourceType,
                ResourceId = resource.ResourceId,
            };
        }

        return new ContentLockStatusDto
        {
            IsLocked = false,
            ResourceType = resource.ResourceType,
            ResourceId = resource.ResourceId,
        };
    }

    public IReadOnlyCollection<ContentLockLifecycleDto> ReleaseAllForConnection(string connectionId)
    {
        var releasedLocks = new List<ContentLockLifecycleDto>();
        foreach (var item in _locksByResource.Where(item => item.Value.ConnectionId == connectionId))
        {
            while (_locksByResource.TryGetValue(item.Key, out var contentLock) && contentLock.ConnectionId == connectionId)
            {
                if (contentLock.IsUpdateInProgress)
                {
                    if (contentLock.ReleaseAfterUpdate)
                    {
                        break;
                    }

                    var releaseAfterUpdate = contentLock with { ReleaseAfterUpdate = true };
                    if (_locksByResource.TryUpdate(item.Key, releaseAfterUpdate, contentLock))
                    {
                        break;
                    }

                    continue;
                }

                if (_locksByResource.TryRemove(new KeyValuePair<string, ContentLock>(item.Key, contentLock)))
                {
                    releasedLocks.Add(ToLifecycle(contentLock));
                }

                break;
            }
        }

        return releasedLocks;
    }

    public IReadOnlyCollection<ContentLockLifecycleDto> ReleaseExpired()
    {
        var now = _timeProvider.GetUtcNow();
        return _locksByResource
            .Where(item => IsExpired(item.Value, now))
            .Where(item => _locksByResource.TryRemove(new KeyValuePair<string, ContentLock>(item.Key, item.Value)))
            .Select(item => ToLifecycle(item.Value))
            .ToArray();
    }

    private static bool Matches(ContentLock contentLock, string lockId, string ownerIdentity, string connectionId)
        => contentLock.LockId == lockId
           && contentLock.OwnerIdentity == ownerIdentity
           && contentLock.ConnectionId == connectionId;

    private static bool IsExpired(ContentLock contentLock, DateTimeOffset now)
        => !contentLock.IsUpdateInProgress && contentLock.ExpiresAtUtc <= now;

    private static ContentLockAcquisitionDto Denied(ContentLockResourceDto resource)
        => new()
        {
            Acquired = false,
            ResourceType = resource.ResourceType,
            ResourceId = resource.ResourceId,
        };

    private static ContentLockReleaseResult DeniedRelease() => new();

    private static ContentLockReleaseResult AcceptedRelease(ContentLockLifecycleDto? releasedLock = null)
        => new()
        {
            Accepted = true,
            ReleasedLock = releasedLock,
        };

    private static ContentLockAcquisitionDto ToAcquisition(ContentLock contentLock, bool acquired)
        => new()
        {
            Acquired = acquired,
            LockId = contentLock.LockId,
            ExpiresAtUtc = contentLock.ExpiresAtUtc,
            ResourceType = contentLock.Resource.ResourceType,
            ResourceId = contentLock.Resource.ResourceId,
        };

    private static ContentLockLifecycleDto ToLifecycle(ContentLock contentLock)
        => new()
        {
            ResourceType = contentLock.Resource.ResourceType,
            ResourceId = contentLock.Resource.ResourceId,
            LockId = contentLock.LockId,
            OwnerIdentity = contentLock.OwnerIdentity,
            ExpiresAtUtc = contentLock.ExpiresAtUtc,
        };

    private sealed record ContentLock(
        string LockId,
        ContentLockResourceDto Resource,
        string OwnerIdentity,
        string ConnectionId,
        DateTimeOffset AcquiredAtUtc,
        DateTimeOffset ExpiresAtUtc,
        bool IsUpdateInProgress = false,
        bool ReleaseAfterUpdate = false);
}
