using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

public class ReportFundsExpendituresSettingsLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public string DisclaimerTitle { get; set; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
