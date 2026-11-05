namespace VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

public record ContentEntityChangedDto
{
    public string ResourceType { get; init; } = string.Empty;

    public long ResourceId { get; init; }
}
