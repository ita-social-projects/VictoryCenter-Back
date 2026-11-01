-- Inputs: @OperationId uniqueidentifier, @Actor nvarchar(200).
-- Run after inspecting and fixing the recorded failure. Never reset active work.
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF NULLIF(LTRIM(RTRIM(@Actor)), N'') IS NULL
    THROW 51000, 'An operator identity is required.', 1;

-- Status=3: Failed; Status=0: Pending. Values follow the persisted enum.
UPDATE media.StorageOperations
SET Status = 0,
    Attempts = 0,
    NextAttemptAt = SYSUTCDATETIME(),
    RequestedBy = @Actor,
    Version = NEWID()
WHERE Id = @OperationId AND Status = 3;

IF @@ROWCOUNT = 0
    THROW 51002, 'Only an existing failed operation can be retried.', 1;
