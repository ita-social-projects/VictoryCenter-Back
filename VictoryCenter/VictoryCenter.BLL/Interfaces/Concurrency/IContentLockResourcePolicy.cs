using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

namespace VictoryCenter.BLL.Interfaces.Concurrency;

public interface IContentLockResourcePolicy
{
    bool IsSupported(ContentLockResourceDto resource);
}
