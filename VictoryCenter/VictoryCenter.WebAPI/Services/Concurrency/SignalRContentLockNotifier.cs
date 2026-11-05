using Microsoft.AspNetCore.SignalR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.WebAPI.Hubs;

namespace VictoryCenter.WebAPI.Services.Concurrency;

public sealed class SignalRContentLockNotifier : IContentLockNotifier
{
    private readonly IHubContext<AdminContentHub> _hubContext;

    public SignalRContentLockNotifier(IHubContext<AdminContentHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyLockReleasedAsync(ContentLockLifecycleDto contentLock, CancellationToken cancellationToken = default)
        => SendAsync(ContentLockEvents.Released, contentLock, cancellationToken);

    public Task NotifyLockExpiredAsync(ContentLockLifecycleDto contentLock, CancellationToken cancellationToken = default)
        => SendAsync(ContentLockEvents.Expired, contentLock, cancellationToken);

    public Task NotifyEntityChangedAsync(ContentEntityChangedDto entityChanged, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(new ContentLockResourceDto
        {
            ResourceType = entityChanged.ResourceType,
            ResourceId = entityChanged.ResourceId,
        }.Key)
            .SendAsync(ContentLockEvents.EntityChanged, entityChanged, cancellationToken);

    private Task SendAsync(string eventName, ContentLockLifecycleDto contentLock, CancellationToken cancellationToken)
    {
        return _hubContext.Clients.Group(new ContentLockResourceDto
        {
            ResourceType = contentLock.ResourceType,
            ResourceId = contentLock.ResourceId,
        }.Key)
            .SendAsync(eventName, contentLock, cancellationToken);
    }
}
