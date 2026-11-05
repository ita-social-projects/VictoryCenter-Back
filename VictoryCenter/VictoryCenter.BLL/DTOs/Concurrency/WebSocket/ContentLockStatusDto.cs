namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentLockStatusDto
{
    public bool IsLocked { get; init; }

    public bool IsUpdateInProgress { get; init; }

    public string? OwnerIdentity { get; init; }

    public DateTimeOffset? AcquiredAtUtc { get; init; }

    public DateTimeOffset? ExpiresAtUtc { get; init; }

    public string ResourceType { get; init; } = string.Empty;

    public long ResourceId { get; init; }
}
