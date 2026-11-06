using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;

namespace VictoryCenter.BLL.Services.Concurrency;

public sealed class ContentLockUpdateCompletionService : IContentLockUpdateCompletionService
{
    private readonly IContentLockNotifier _notifier;
    private readonly ILogger<ContentLockUpdateCompletionService> _logger;

    public ContentLockUpdateCompletionService(
        IContentLockNotifier notifier,
        ILogger<ContentLockUpdateCompletionService> logger)
    {
        _notifier = notifier;
        _logger = logger;
    }

    public async Task CompleteAsync(ContentLockResourceDto resource)
    {
        await NotifySafelyAsync(() => _notifier.NotifyEntityChangedAsync(
            new ContentEntityChangedDto
            {
                ResourceType = resource.ResourceType,
                ResourceId = resource.ResourceId,
            }));
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
