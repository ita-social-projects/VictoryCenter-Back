using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

namespace VictoryCenter.BLL.Interfaces.Concurrency;

public interface IContentLockNotifier
{
    Task NotifyLockReleasedAsync(ContentLockLifecycleDto contentLock, CancellationToken cancellationToken = default);

    Task NotifyLockExpiredAsync(ContentLockLifecycleDto contentLock, CancellationToken cancellationToken = default);

    Task NotifyEntityChangedAsync(ContentEntityChangedDto entityChanged, CancellationToken cancellationToken = default);
}
