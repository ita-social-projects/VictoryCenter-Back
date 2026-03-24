using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

public record CompanyProfileRequisiteLocalizationDto : BaseCompanyProfileRequisiteLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public TranslationStatus TranslationStatus { get; init; }

}
