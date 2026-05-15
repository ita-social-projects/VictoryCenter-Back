using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

public class CreateReportFundsExpendituresSettingsLocalizationDto
    : UpdateReportFundsExpendituresSettingsLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
}
