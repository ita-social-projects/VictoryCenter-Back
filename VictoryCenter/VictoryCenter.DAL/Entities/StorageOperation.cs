namespace VictoryCenter.DAL.Entities;

public enum StorageFileKind
{
    Image,
    Pdf
}

public enum StorageOperationKind
{
    UploadCleanup,
    Delete
}

public enum StorageOperationStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public class StorageOperation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public StorageFileKind FileKind { get; set; }
    public string FileName { get; set; } = null!;
    public StorageOperationKind Kind { get; set; }
    public StorageOperationStatus Status { get; set; }
    public int Attempts { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset NextAttemptAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? LastErrorCode { get; set; }
    public string? RequestedBy { get; set; }
    public Guid Version { get; set; } = Guid.NewGuid();
}
