-- Inputs: @FindingId uniqueidentifier, @Actor nvarchar(200).
-- This queues work only. The worker checks file age, references and active uploads.
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF NULLIF(LTRIM(RTRIM(@Actor)), N'') IS NULL
    THROW 51000, 'An operator identity is required.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @OperationId uniqueidentifier;
    DECLARE @FileKind int;
    DECLARE @FileName nvarchar(150);

    SELECT
        @OperationId = CleanupOperationId,
        @FileKind = FileKind,
        @FileName = FileName
    FROM media.StorageReconciliationFindings WITH (UPDLOCK, HOLDLOCK)
    WHERE Id = @FindingId AND Kind = 1 AND ResolvedAt IS NULL;

    IF @FileName IS NULL
        THROW 51001, 'An unresolved unreferenced-file finding is required.', 1;

    IF @OperationId IS NULL
    BEGIN
        SET @OperationId = NEWID();

        -- Kind=1: Delete; Status=0: Pending. Values follow the persisted enums.
        INSERT INTO media.StorageOperations
            (Id, FileKind, FileName, Kind, Status, Attempts, CreatedAt, NextAttemptAt, RequestedBy, Version)
        VALUES
            (@OperationId, @FileKind, @FileName, 1, 0, 0, SYSUTCDATETIME(), SYSUTCDATETIME(), @Actor, NEWID());

        UPDATE media.StorageReconciliationFindings
        SET CleanupOperationId = @OperationId
        WHERE Id = @FindingId;
    END;

    COMMIT TRANSACTION;
    SELECT @OperationId AS OperationId;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
