namespace VictoryCenter.BLL.DTOs.Images;
public record UpdateImageDTO
{
    public string? Base64 { get; init; }
    public string? MimeType { get; init; }
}
