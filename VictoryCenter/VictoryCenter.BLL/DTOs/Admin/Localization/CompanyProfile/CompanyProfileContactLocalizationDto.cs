using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Migrations;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

public record CompanyProfileContactLocalizationDto : BaseCompanyProfileContactLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public TranslationStatus TranslationStatus { get; init; } = null!;

}
