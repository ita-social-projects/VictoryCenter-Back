using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;

namespace VictoryCenter.BLL.DTOs.Admin.MainPages;

public record CreateMainPageDto : BaseMainPageDto
{
    public CreateMainAboutUsDto? MainAboutUs { get; init; }
    public CreateMainPartnersDto? MainPartners { get; init; }
    public ICollection<CreateImpactStatisticDto> ImpactStatistics { get; init; } = [];
}
