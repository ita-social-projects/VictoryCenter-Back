namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record ReportMediaSettingsDto
{
    public CollectedFundsBlockDto CollectedFundsBlock { get; set; }
    public ChangedLivesBlockDto ChangedLivesBlock { get; set; }
}
