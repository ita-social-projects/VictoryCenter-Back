namespace VictoryCenter.BLL.DTOs.Images;
public record CreateImageDTO
{
    public string Base64 { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public int? TeamMemberId { get; set; }
}
