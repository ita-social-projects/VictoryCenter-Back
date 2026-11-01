-- Read-only queue summary. Values follow the enums in StorageOperation.cs.
-- FileKind: Image=0, Pdf=1; Kind: UploadCleanup=0, Delete=1.
-- Status: Pending=0, Processing=1, Completed=2, Failed=3.
SET NOCOUNT ON;

SELECT
    FileKind,
    Kind,
    Status,
    COUNT_BIG(*) AS OperationCount
FROM media.StorageOperations
GROUP BY FileKind, Kind, Status
ORDER BY FileKind, Kind, Status;
