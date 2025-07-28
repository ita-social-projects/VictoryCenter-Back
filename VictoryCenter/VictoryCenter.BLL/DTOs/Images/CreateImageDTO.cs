namespace VictoryCenter.BLL.DTOs.Images;
public record CreateImageDTO
{
    public string Base64 { get; init; } = null!;
    public string MimeType { get; init; } = null!;
}
