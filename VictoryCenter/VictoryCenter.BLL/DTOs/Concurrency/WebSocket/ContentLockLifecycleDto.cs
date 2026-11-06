namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentLockLifecycleDto
{
    public string ResourceType { get; init; } = string.Empty;

    public long ResourceId { get; init; }

    public string LockId { get; init; } = string.Empty;

    public string OwnerIdentity { get; init; } = string.Empty;

    public DateTimeOffset? ExpiresAtUtc { get; init; }
}
