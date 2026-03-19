namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

public abstract record BaseReportFundsExpendituresSettingsDto
{
    public string DisclaimerTitle { get; init; } = null!;
    public decimal ExchangeRate { get; init; }
}
