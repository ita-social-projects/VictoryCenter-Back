namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnerDto : BasePartnerCreateUpdateDto
{
    public long? Id { get; init; }
}

/*public record UpdatePartnerDto : BasePartnerCreateUpdateDto
{
    public long Id { get; init; }
    public UpdatePartnerImageDto? Image { get; init; } = null!;
}
*/
