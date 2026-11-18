using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public string? ActorId { get; set; }
    public AuditOperation Operation { get; set; }
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public string? Changes { get; set; }
}
