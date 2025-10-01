namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record CreatePartnerImageDto
{
    public string Base64 { get; init; } = null!;
    public string MimeType { get; init; } = null!;
}
