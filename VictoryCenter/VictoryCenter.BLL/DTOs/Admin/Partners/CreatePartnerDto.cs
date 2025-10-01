namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record CreatePartnerDto
{
    public string Description { get; init; } = null!;
    public CreatePartnerImageDto Image { get; init; } = null!;
}
