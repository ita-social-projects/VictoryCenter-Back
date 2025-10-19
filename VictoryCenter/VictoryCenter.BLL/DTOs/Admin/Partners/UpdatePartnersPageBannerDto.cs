namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnersPageBannerDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public long? ImageId { get; init; }

}
