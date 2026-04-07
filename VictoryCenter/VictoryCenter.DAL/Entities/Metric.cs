using VictoryCenter.DAL.Data.BaseEntity;
namespace VictoryCenter.DAL.Entities;

public class Metric : BaseEntity
{
    public long StatisticId { get; set; }
    public ImpactStatistics Statistics { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Signature { get; set; } = null!;
}
