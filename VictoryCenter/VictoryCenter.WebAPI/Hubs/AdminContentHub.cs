using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;

namespace VictoryCenter.WebAPI.Hubs;

[Authorize]
public sealed class AdminContentHub : Hub
{
    private readonly IContentLockManager _lockManager;
    private readonly IContentLockResourcePolicy _resourcePolicy;

    public AdminContentHub(IContentLockManager lockManager, IContentLockResourcePolicy resourcePolicy)
    {
        _lockManager = lockManager;
        _resourcePolicy = resourcePolicy;
    }

    public async Task<ContentLockAcquisitionDto> AcquireLock(ContentLockResourceDto resource)
    {
        EnsureSupported(resource);
        await Groups.AddToGroupAsync(Context.ConnectionId, resource.Key);
        var ownerIdentity = GetOwnerIdentity();
        var result = _lockManager.Acquire(resource, ownerIdentity, Context.ConnectionId);
        if (result.Acquired)
        {
            await Clients.Group(resource.Key).SendAsync(ContentLockEvents.Acquired, ToLifecycle(result, ownerIdentity));
        }
        else
        {
            await Clients.Caller.SendAsync(ContentLockEvents.Denied, _lockManager.GetStatus(resource));
        }

        return result;
    }

    public Task<ContentLockAcquisitionDto> RenewLock(ContentLockResourceDto resource, string lockId)
    {
        EnsureSupported(resource);
        return Task.FromResult(_lockManager.Renew(lockId, resource, GetOwnerIdentity(), Context.ConnectionId));
    }

    public async Task<bool> ReleaseLock(ContentLockResourceDto resource, string lockId)
    {
        EnsureSupported(resource);
        var ownerIdentity = GetOwnerIdentity();
        var releaseResult = _lockManager.Release(lockId, resource, ownerIdentity, Context.ConnectionId);
        if (releaseResult.ReleasedLock is { } releasedLock)
        {
            await Clients.Group(resource.Key).SendAsync(
                ContentLockEvents.Released,
                releasedLock);
        }

        return releaseResult.Accepted;
    }

    public async Task<ContentLockStatusDto> GetLockStatus(ContentLockResourceDto resource)
    {
        EnsureSupported(resource);
        await Groups.AddToGroupAsync(Context.ConnectionId, resource.Key);
        return _lockManager.GetStatus(resource);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var released in _lockManager.ReleaseAllForConnection(Context.ConnectionId))
        {
            await Clients.Group(new ContentLockResourceDto
            {
                ResourceType = released.ResourceType,
                ResourceId = released.ResourceId,
            }.Key)
                .SendAsync(ContentLockEvents.Released, released);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private string GetOwnerIdentity()
        => Context.User?.FindFirstValue(ClaimTypes.Email)
           ?? throw new HubException(ContentLockConstants.AuthenticatedAdministratorIdentityRequiredMessage);

    private static ContentLockLifecycleDto ToLifecycle(ContentLockAcquisitionDto result, string ownerIdentity)
        => new()
        {
            ResourceType = result.ResourceType,
            ResourceId = result.ResourceId,
            LockId = result.LockId!,
            OwnerIdentity = ownerIdentity,
            ExpiresAtUtc = result.ExpiresAtUtc,
        };

    private void EnsureSupported(ContentLockResourceDto resource)
    {
        if (!_resourcePolicy.IsSupported(resource))
        {
            throw new HubException(ContentLockConstants.UnsupportedResourceMessage);
        }
    }
}
