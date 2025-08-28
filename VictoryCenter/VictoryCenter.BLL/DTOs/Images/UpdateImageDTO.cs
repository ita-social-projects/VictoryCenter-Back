namespace VictoryCenter.BLL.DTOs.Images;
public record UpdateImageDto
{
    public string? Base64 { get; init; }
    public string? MimeType { get; init; }
}
