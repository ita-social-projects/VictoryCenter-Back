using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record ReorderPartnersDto : BaseReorderDto
{
    public long PartnersSectionId { get; init; }
}
