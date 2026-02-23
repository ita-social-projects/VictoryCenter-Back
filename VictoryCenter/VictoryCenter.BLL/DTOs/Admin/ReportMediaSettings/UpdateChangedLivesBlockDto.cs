namespace VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

public record UpdateChangedLivesBlockDto
{
    public string Title { get; init; } = null!;
    public int ChangedLives { get; init; }
    public long ImageId { get; init; }
}
