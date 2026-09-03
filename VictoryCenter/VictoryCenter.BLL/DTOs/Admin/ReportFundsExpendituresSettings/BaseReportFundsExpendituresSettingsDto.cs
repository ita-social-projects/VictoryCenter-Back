using System.Text.Json.Serialization;
using VictoryCenter.BLL.Helpers;
namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

public abstract record BaseReportFundsExpendituresSettingsDto
{
    [JsonConverter(typeof(TrimStringJsonHelper))]
    public string DisclaimerTitle { get; init; } = null!;
    public decimal ExchangeRate { get; init; }
    public int ProgramExpendituresReportingYear { get; init; }
}
