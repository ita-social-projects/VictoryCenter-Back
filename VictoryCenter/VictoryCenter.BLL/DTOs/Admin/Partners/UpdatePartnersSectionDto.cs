namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnersSectionDto : BasePartnerSectionCreateUpdateDto
{
    public List<UpdatePartnerDto> Partners { get; init; } = [];
    public List<long> PartnerIdsToDelete { get; init; } = [];
}
