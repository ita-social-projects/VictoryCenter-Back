using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class StorageOperationProcessor(
    VictoryCenterDbContext context,
    LocalStorageFiles files,
    IOptions<StorageLifecycleOptions> options,
    TimeProvider clock,
    ILogger<StorageOperationProcessor> logger)
{
    public async Task<int> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        var ids = await context.StorageOperations.AsNoTracking()
            .Where(operation => (operation.Status == StorageOperationStatus.Pending
                || operation.Status == StorageOperationStatus.Processing) && operation.NextAttemptAt <= now)
            .OrderBy(operation => operation.NextAttemptAt).ThenBy(operation => operation.Id)
            .Select(operation => operation.Id).Take(options.Value.BatchSize).ToListAsync(cancellationToken);

        var processed = 0;
        foreach (var id in ids)
        {
            context.ChangeTracker.Clear();
            try
            {
                processed += await ProcessOperationAsync(id, cancellationToken) ? 1 : 0;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // The persisted lease permits retry even when acknowledgement could not be saved.
                context.ChangeTracker.Clear();
                logger.LogError(
                    "Storage operation {OperationId} failed unexpectedly with {ErrorCode}; later operations will continue",
                    id,
                    exception.GetType().Name);
            }
        }

        return processed;
    }

    private async Task<bool> ProcessOperationAsync(Guid id, CancellationToken cancellationToken)
    {
        var operation = await context.StorageOperations.SingleAsync(item => item.Id == id, cancellationToken);
        if (operation.Status is StorageOperationStatus.Completed or StorageOperationStatus.Failed
            || operation.NextAttemptAt > clock.GetUtcNow())
        {
            return false;
        }

        if (operation.Attempts >= options.Value.MaxAttempts)
        {
            operation.Status = StorageOperationStatus.Failed;
            operation.LastErrorCode = StorageLifecycleConstants.RetryLimitReached;
            operation.Version = Guid.NewGuid();
        }
        else
        {
            operation.Status = StorageOperationStatus.Processing;
            operation.Attempts++;
            operation.NextAttemptAt = clock.GetUtcNow().AddSeconds(options.Value.LeaseSeconds);
            operation.Version = Guid.NewGuid();
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another worker or the metadata commit acquired the same intent.
            context.ChangeTracker.Clear();
            return false;
        }

        if (operation.Status == StorageOperationStatus.Failed)
        {
            logger.LogError("Storage operation {OperationId} exhausted its retry budget", operation.Id);
            return false;
        }

        await ProcessClaimedAsync(operation, cancellationToken);
        return true;
    }

    private async Task ProcessClaimedAsync(StorageOperation operation, CancellationToken cancellationToken)
    {
        try
        {
            var referenced = await StorageReferences.ExistsAsync(context, operation.FileKind, operation.FileName, cancellationToken);
            if (referenced)
            {
                // Upload cleanup can encounter legacy callers that persisted metadata before uploading.
                operation.Status = operation.Kind == StorageOperationKind.UploadCleanup
                    ? StorageOperationStatus.Completed : StorageOperationStatus.Failed;
                operation.LastErrorCode = operation.Kind == StorageOperationKind.Delete ? StorageLifecycleConstants.FileStillReferenced : null;
            }
            else if (operation.Kind == StorageOperationKind.Delete && !await IsCleanupSafeAsync(operation, cancellationToken))
            {
                operation.Status = StorageOperationStatus.Failed;
                operation.LastErrorCode = StorageLifecycleConstants.CleanupNotSafe;
            }
            else
            {
                DeleteFiles(operation);
                operation.Status = StorageOperationStatus.Completed;
                operation.LastErrorCode = null;
            }

            operation.CompletedAt = operation.Status == StorageOperationStatus.Completed ? clock.GetUtcNow() : null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            operation.LastErrorCode = $"{exception.GetType().Name}:{exception.HResult:X8}";
            operation.Status = operation.Attempts >= options.Value.MaxAttempts
                ? StorageOperationStatus.Failed : StorageOperationStatus.Pending;
            operation.NextAttemptAt = clock.GetUtcNow().AddSeconds(Math.Min(
                options.Value.MaxRetryDelaySeconds,
                options.Value.RetryDelaySeconds * Math.Pow(2, operation.Attempts - 1)));
            logger.LogWarning(
                "Storage operation {OperationId} for {FileKind} failed with {ErrorCode}; attempt {Attempt}",
                operation.Id,
                operation.FileKind,
                operation.LastErrorCode,
                operation.Attempts);
        }

        operation.Version = Guid.NewGuid();
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            context.ChangeTracker.Clear();
            logger.LogWarning("Storage operation {OperationId} was reclaimed before acknowledgement", operation.Id);
        }

        logger.LogInformation("Storage operation {OperationId} finished attempt with status {Status}", operation.Id, operation.Status);
    }

    private void DeleteFiles(StorageOperation operation)
    {
        File.Delete(files.GetPath(operation.FileKind, operation.FileName));
        if (operation.Kind != StorageOperationKind.UploadCleanup)
        {
            return;
        }

        foreach (var temporaryFile in files.Enumerate(operation.FileKind)
            .Where(name => StorageFileNames.TemporaryOwner(name) == operation.FileName))
        {
            File.Delete(files.GetPath(operation.FileKind, temporaryFile));
        }
    }

    private async Task<bool> IsCleanupSafeAsync(StorageOperation operation, CancellationToken cancellationToken)
    {
        var finding = await context.StorageReconciliationFindings.AsNoTracking()
            .SingleOrDefaultAsync(item => item.CleanupOperationId == operation.Id, cancellationToken);
        if (finding == null)
        {
            return true;
        }

        if (finding.FileKind != operation.FileKind || !string.Equals(finding.FileName, operation.FileName, StringComparison.Ordinal))
        {
            return false;
        }

        if (!files.Exists(operation.FileKind, operation.FileName))
        {
            return true;
        }

        var cutoff = clock.GetUtcNow().AddHours(-options.Value.OrphanMinimumAgeHours);
        if (finding.Kind != StorageFindingKind.Unreferenced || finding.ResolvedAt != null || finding.FirstSeenAt > cutoff)
        {
            return false;
        }

        return File.GetLastWriteTimeUtc(files.GetPath(operation.FileKind, operation.FileName)) <= cutoff.UtcDateTime
            && !await StorageReferences.IsProtectedAsync(context, operation.FileKind, operation.FileName, cancellationToken, operation.Id);
    }
}
