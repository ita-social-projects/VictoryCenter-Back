namespace VictoryCenter.BLL.DTOs.Admin.Images;

public record UpdateImageDto
{
    public string? Base64 { get; init; }
    public string? MimeType { get; init; }
}
