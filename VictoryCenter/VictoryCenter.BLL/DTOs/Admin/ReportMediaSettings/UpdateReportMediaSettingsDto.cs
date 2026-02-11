namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record UpdateReportMediaSettingsDto
{
    public UpdateCollectedFundsBlockDto CollectedFundsBlock { get; set; }
    public UpdateChangedLivesBlockDto ChangedLivesBlock { get; set; }
}
