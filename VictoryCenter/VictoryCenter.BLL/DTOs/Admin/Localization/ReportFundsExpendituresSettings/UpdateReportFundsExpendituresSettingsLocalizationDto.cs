using System.Text.Json.Serialization;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

public class UpdateReportFundsExpendituresSettingsLocalizationDto
{
    [JsonConverter(typeof(TrimStringJsonHelper))]
    public string DisclaimerTitle { get; set; } = null!;
}
