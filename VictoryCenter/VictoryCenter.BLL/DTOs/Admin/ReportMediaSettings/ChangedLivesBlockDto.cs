using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record ChangedLivesBlockDto
{
    public string Title { get; init; } = null!;
    public int ChangedLives { get; init; }
    public ImageDto? Image { get; init; }
}
