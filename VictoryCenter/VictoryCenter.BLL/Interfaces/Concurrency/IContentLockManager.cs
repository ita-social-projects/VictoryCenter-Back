using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

namespace VictoryCenter.BLL.Interfaces.Concurrency;

public interface IContentLockManager
{
    ContentLockAcquisitionDto Acquire(ContentLockResourceDto resource, string ownerIdentity, string connectionId);

    ContentLockAcquisitionDto Renew(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId);

    ContentLockReleaseResult Release(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId);

    bool TryBeginUpdate(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId);

    ContentLockLifecycleDto? EndUpdate(string lockId, ContentLockResourceDto resource, string ownerIdentity, string connectionId);

    ContentLockStatusDto GetStatus(ContentLockResourceDto resource);

    IReadOnlyCollection<ContentLockLifecycleDto> ReleaseAllForConnection(string connectionId);

    IReadOnlyCollection<ContentLockLifecycleDto> ReleaseExpired();
}
