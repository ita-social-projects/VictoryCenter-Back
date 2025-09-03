namespace VictoryCenter.BLL.DTOs.Admin.Images;

public record CreateImageDto
{
    public string Base64 { get; init; } = null!;
    public string MimeType { get; init; } = null!;
}
