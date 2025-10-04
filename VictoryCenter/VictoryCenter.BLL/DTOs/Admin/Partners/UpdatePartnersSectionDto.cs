namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnersSectionDto : BasePartnerSectionCreateUpdateDto<UpdatePartnerDto>
{
    public List<long> PartnerIdsToDelete { get; init; } = [];
}
