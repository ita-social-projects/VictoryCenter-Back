namespace VictoryCenter.BLL.DTOs.Images;
public record UpdateImageDTO
{
    public string? Base64 { get; set; }
    public string? MimeType { get; set; }
}
