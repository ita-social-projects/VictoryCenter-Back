namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentLockAcquisitionDto
{
    public bool Acquired { get; init; }

    public string? LockId { get; init; }

    public DateTimeOffset? ExpiresAtUtc { get; init; }

    public string ResourceType { get; init; } = string.Empty;

    public long ResourceId { get; init; }
}
