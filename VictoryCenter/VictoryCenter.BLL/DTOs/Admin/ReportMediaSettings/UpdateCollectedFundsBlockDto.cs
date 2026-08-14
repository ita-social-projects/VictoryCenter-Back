namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record UpdateCollectedFundsBlockDto
{
    public string Title { get; init; } = null!;
    public string TitleEn { get; init; } = null!;
    public long? ImageId { get; init; }
}
