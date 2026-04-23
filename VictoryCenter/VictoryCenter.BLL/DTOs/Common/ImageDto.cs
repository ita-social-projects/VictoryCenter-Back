namespace VictoryCenter.BLL.DTOs.Common;

public record ImageDto
{
    public long Id { get; init; }
    public string BlobName { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string MimeType { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}
