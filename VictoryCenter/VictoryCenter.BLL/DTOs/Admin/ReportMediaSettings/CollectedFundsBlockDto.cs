using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record CollectedFundsBlockDto
{
    public string Title { get; init; } = null!;
    public int CollectedAmount { get; init; }
    public ImageDto? Image { get; init; }
}
