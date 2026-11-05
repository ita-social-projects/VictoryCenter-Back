using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;

namespace VictoryCenter.BLL.Services.Concurrency;

public sealed class ContentLockUpdateCompletionService : IContentLockUpdateCompletionService
{
    private readonly IContentLockManager _lockManager;
    private readonly IContentLockNotifier _notifier;
    private readonly ILogger<ContentLockUpdateCompletionService> _logger;

    public ContentLockUpdateCompletionService(
        IContentLockManager lockManager,
        IContentLockNotifier notifier,
        ILogger<ContentLockUpdateCompletionService> logger)
    {
        _lockManager = lockManager;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task CompleteAsync(
        ContentLockResourceDto resource,
        ContentLockSessionDto? session)
    {
        var releaseResult = session is null
            ? null
            : _lockManager.Release(session.LockId, resource, session.OwnerIdentity, session.ConnectionId);
        await NotifySafelyAsync(() => _notifier.NotifyEntityChangedAsync(
            new ContentEntityChangedDto
            {
                ResourceType = resource.ResourceType,
                ResourceId = resource.ResourceId,
            }));

        if (releaseResult?.ReleasedLock is { } releasedLock)
        {
            await NotifySafelyAsync(() => _notifier.NotifyLockReleasedAsync(releasedLock));
        }
    }

    private async Task NotifySafelyAsync(Func<Task> notify)
    {
        try
        {
            await notify();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to publish a content-lock notification after content was persisted.");
        }
    }
}
