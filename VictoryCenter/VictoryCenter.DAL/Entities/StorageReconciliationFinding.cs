namespace VictoryCenter.DAL.Entities;

public enum StorageFindingKind
{
    Missing,
    Unreferenced
}

public class StorageReconciliationFinding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public StorageFileKind FileKind { get; set; }
    public string FileName { get; set; } = null!;
    public StorageFindingKind Kind { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public Guid? CleanupOperationId { get; set; }
}
