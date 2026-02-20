namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record UpdateCollectedFundsBlockDto
{
    public string Title { get; init; } = null!;
    public long CollectedFunds { get; init; }
    public long ImageId { get; init; }
}
