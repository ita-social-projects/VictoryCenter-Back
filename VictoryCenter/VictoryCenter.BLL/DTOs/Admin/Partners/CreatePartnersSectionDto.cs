namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record CreatePartnersSectionDto : BasePartnerSectionCreateUpdateDto
{
    public List<CreatePartnerDto> Partners { get; init; } = [];
}
