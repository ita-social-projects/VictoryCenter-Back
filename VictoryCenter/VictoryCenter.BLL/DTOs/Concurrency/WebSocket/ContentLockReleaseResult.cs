namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentLockReleaseResult
{
    public bool Accepted { get; init; }

    public ContentLockLifecycleDto? ReleasedLock { get; init; }
}
