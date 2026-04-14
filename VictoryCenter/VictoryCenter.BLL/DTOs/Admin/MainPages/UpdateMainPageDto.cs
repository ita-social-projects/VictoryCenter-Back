using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

namespace VictoryCenter.BLL.DTOs.Admin.MainPages;

public record UpdateMainPageDto : BaseMainPageDto
{
    public UpdateMainAboutUsDto? MainAboutUs { get; init; }
    public UpdateMainPartnersDto? MainPartners { get; init; }
    public ICollection<UpdateImpactStatisticDto> ImpactStatistics { get; init; } = [];
}