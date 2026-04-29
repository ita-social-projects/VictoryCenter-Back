namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public abstract record BaseImpactStatisticDto
{
    public string Title { get; init; } = null!;
    public long? ImageId { get; init; }
}
