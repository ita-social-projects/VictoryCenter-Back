namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnerDto : BasePartnerCreateUpdateDto
{
    public long? Id { get; init; } = null;
    public UpdatePartnerImageDto Image { get; init; } = null!;
}
