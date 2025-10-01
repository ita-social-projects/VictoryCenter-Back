namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnersSectionDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<UpdatePartnerDto> Partners { get; init; } = [];
    public List<long> PartnerIdsToDelete { get; init; } = [];
}
