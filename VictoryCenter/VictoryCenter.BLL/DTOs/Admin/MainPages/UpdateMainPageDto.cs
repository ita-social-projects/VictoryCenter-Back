using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

namespace VictoryCenter.BLL.DTOs.Admin.MainPages;

public record UpdateMainPageDto : BaseMainPageDto
{
    public UpdateMainAboutUsDto? MainAboutUs { get; init; }
    public UpdateMainPartnersDto? MainPartners { get; init; }
    public UpdateMainDonationsDto? MainDonations { get; init; }
    public UpdateImpactStatisticDto? ImpactStatistics { get; init; }
}
