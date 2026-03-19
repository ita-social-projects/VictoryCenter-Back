namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

public record ReportFundsExpendituresSettingsDto
{
    public long Id { get; init; }
    public string DisclaimerTitle { get; init; } = null!;
    public decimal ExchangeRate { get; init; }
}
