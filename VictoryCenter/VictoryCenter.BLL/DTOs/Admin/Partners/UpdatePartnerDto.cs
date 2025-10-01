namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnerDto
{
    public long? Id { get; init; } = null;
    public string Description { get; init; } = null!;
    public UpdatePartnerImageDto Image { get; init; } = null!;
}
