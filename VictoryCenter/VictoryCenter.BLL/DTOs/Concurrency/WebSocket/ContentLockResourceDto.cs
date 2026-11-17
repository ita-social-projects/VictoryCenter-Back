namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentLockResourceDto
{
    public string ResourceType { get; init; } = string.Empty;

    public long ResourceId { get; init; }

    public string Key => $"{ResourceType}:{ResourceId}";
}
