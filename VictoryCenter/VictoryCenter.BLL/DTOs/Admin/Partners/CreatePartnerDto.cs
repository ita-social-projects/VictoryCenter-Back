namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record CreatePartnerDto : BasePartnerCreateUpdateDto
{
    public CreatePartnerImageDto Image { get; init; } = null!;
}
