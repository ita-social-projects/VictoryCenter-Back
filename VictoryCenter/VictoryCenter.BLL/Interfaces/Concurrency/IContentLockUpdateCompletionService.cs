using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

namespace VictoryCenter.BLL.Interfaces.Concurrency;

public interface IContentLockUpdateCompletionService
{
    Task CompleteAsync(ContentLockResourceDto resource);
}
