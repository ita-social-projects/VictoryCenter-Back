namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record PartnersSectionDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<PartnerDto> Partners { get; init; } = [];
}
