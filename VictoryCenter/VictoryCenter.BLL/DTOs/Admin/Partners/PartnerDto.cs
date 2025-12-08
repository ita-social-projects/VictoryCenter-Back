using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record PartnerDto
{
    public long Id { get; init; }
    public string Description { get; init; }
    public ImageDto Image { get; init; } = null!;
}
