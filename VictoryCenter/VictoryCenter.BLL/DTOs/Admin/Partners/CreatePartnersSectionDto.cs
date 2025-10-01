namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record CreatePartnersSectionDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<CreatePartnerDto> Partners { get; init; } = [];
}
