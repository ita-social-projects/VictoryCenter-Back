namespace VictoryCenter.BLL.DTOs.Images;
public record ImageDTO
{
    public int Id { get; set; }
    public string BlobName { get; set; } = null!;
    public string Base64 { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public int? TeamMemberId { get; set; }
}
