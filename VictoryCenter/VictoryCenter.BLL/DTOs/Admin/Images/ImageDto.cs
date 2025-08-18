namespace VictoryCenter.BLL.DTOs.Admin.Images;

public record ImageDto
{
    public long Id { get; init; }
    public string BlobName { get; init; } = null!;
    public string Base64 { get; set; } = null!;
    public string MimeType { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}
