namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentLockSessionDto
{
    public string LockId { get; init; } = string.Empty;

    public string OwnerIdentity { get; init; } = string.Empty;

    public string ConnectionId { get; init; } = string.Empty;
}
