using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.MainPages;

public record MainPageDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; }

    public MainAboutUsDto? MainAboutUs { get; init; }
    public MainPartnersDto? MainPartners { get; init; }
    public ImpactStatisticDto? ImpactStatistics { get; init; }
}
