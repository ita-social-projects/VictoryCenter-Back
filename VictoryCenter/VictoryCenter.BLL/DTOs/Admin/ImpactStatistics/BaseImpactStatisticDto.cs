namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public abstract record BaseImpactStatisticDto
{
    public string Description { get; init; } = null!;
    public long? ImageId { get; init; }
}
