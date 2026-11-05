using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;

namespace VictoryCenter.WebAPI.Services.Concurrency;

public sealed class ContentLockResourcePolicy : IContentLockResourcePolicy
{
    public bool IsSupported(ContentLockResourceDto resource)
        => resource.ResourceId > 0
           && string.Equals(resource.ResourceType, ContentLockConstants.EventNewsResourceType, StringComparison.Ordinal);
}
