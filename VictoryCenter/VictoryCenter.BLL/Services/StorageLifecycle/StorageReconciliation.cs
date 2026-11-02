using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class StorageReconciliation(
    VictoryCenterDbContext context,
    LocalStorageFiles files,
    TimeProvider clock,
    ILogger<StorageReconciliation> logger)
{
    private const int ScanBatchSize = 200;

    public async Task<int> ScanAsync(CancellationToken cancellationToken)
    {
        var scanStartedAt = clock.GetUtcNow();
        var findings = 0;
        var failures = 0;
        foreach (var fileKind in Enum.GetValues<StorageFileKind>())
        {
            files.EnsureAvailable(fileKind);
        }

        foreach (var fileKind in Enum.GetValues<StorageFileKind>())
        {
            var (referenceFindings, referenceFailures) = await ScanReferencesAsync(fileKind, cancellationToken);
            var (inventoryFindings, inventoryFailures) = await ScanFilesAsync(fileKind, cancellationToken);
            findings += referenceFindings + inventoryFindings;
            failures += referenceFailures + inventoryFailures;
        }

        failures += await RecheckStaleFindingsAsync(scanStartedAt, cancellationToken);
        if (failures > 0)
        {
            logger.LogWarning("Storage reconciliation skipped {FailureCount} entries because of storage errors", failures);
        }

        return findings;
    }

    private async Task<(int Findings, int Failures)> ScanReferencesAsync(StorageFileKind fileKind, CancellationToken cancellationToken)
    {
        var findings = 0;
        var failures = 0;
        long afterId = 0;
        List<StorageReference> references;
        do
        {
            references = await StorageReferences.Inventory(context, fileKind)
                .Where(reference => reference.Id > afterId).OrderBy(reference => reference.Id)
                .Take(ScanBatchSize).ToListAsync(cancellationToken);
            foreach (var reference in references)
            {
                try
                {
                    var name = fileKind == StorageFileKind.Image
                        ? StorageFileNames.Image(reference.Name, reference.MimeType) : reference.Name;
                    var missing = !files.Exists(fileKind, name);
                    await RecordAsync(fileKind, name, StorageFindingKind.Missing, missing, cancellationToken);
                    findings += missing ? 1 : 0;
                }
                catch (Exception exception) when (IsEntryError(exception))
                {
                    failures++;
                    logger.LogWarning(
                        "Storage reconciliation skipped reference {ReferenceId} of {FileKind} with {ErrorCode}",
                        reference.Id,
                        fileKind,
                        exception.GetType().Name);
                }
            }

            afterId = references.Count > 0 ? references[^1].Id : afterId;
        }
        while (references.Count == ScanBatchSize);

        return (findings, failures);
    }

    private async Task<(int Findings, int Failures)> ScanFilesAsync(StorageFileKind fileKind, CancellationToken cancellationToken)
    {
        var findings = 0;
        var failures = 0;
        foreach (var fileName in files.Enumerate(fileKind))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var unreferenced = !await StorageReferences.ExistsAsync(context, fileKind, fileName, cancellationToken);
                if (unreferenced && await StorageReferences.IsProtectedAsync(context, fileKind, fileName, cancellationToken))
                {
                    // Pending cleanup must not resolve its own finding before execution.
                    continue;
                }

                await RecordAsync(fileKind, fileName, StorageFindingKind.Unreferenced, unreferenced, cancellationToken);
                findings += unreferenced ? 1 : 0;
            }
            catch (Exception exception) when (IsEntryError(exception))
            {
                failures++;
                logger.LogWarning("Storage reconciliation skipped a file of {FileKind} with {ErrorCode}", fileKind, exception.GetType().Name);
            }
        }

        return (findings, failures);
    }

    private async Task<int> RecheckStaleFindingsAsync(DateTimeOffset scanStartedAt, CancellationToken cancellationToken)
    {
        HashSet<Guid> failedIds = [];
        List<StorageReconciliationFinding> staleFindings;
        do
        {
            staleFindings = await context.StorageReconciliationFindings.AsNoTracking()
                .Where(finding => finding.ResolvedAt == null && finding.LastSeenAt < scanStartedAt && !failedIds.Contains(finding.Id))
                .OrderBy(finding => finding.LastSeenAt).ThenBy(finding => finding.Id)
                .Take(ScanBatchSize).ToListAsync(cancellationToken);
            foreach (var finding in staleFindings)
            {
                try
                {
                    var referenced = await StorageReferences.ExistsAsync(context, finding.FileKind, finding.FileName, cancellationToken);
                    var exists = files.Exists(finding.FileKind, finding.FileName);
                    var stillPresent = finding.Kind == StorageFindingKind.Missing ? referenced && !exists : !referenced && exists;
                    await RecordAsync(finding.FileKind, finding.FileName, finding.Kind, stillPresent, cancellationToken);
                }
                catch (Exception exception) when (IsEntryError(exception))
                {
                    failedIds.Add(finding.Id);
                    logger.LogWarning("Storage reconciliation skipped finding {FindingId} with {ErrorCode}", finding.Id, exception.GetType().Name);
                }
            }
        }
        while (staleFindings.Count == ScanBatchSize);

        return failedIds.Count;
    }

    private static bool IsEntryError(Exception exception)
    {
        if (exception is DirectoryNotFoundException)
        {
            return false;
        }

        return exception is IOException or UnauthorizedAccessException or ArgumentException;
    }

    private async Task RecordAsync(
        StorageFileKind fileKind,
        string fileName,
        StorageFindingKind kind,
        bool present,
        CancellationToken cancellationToken)
    {
        files.EnsureAvailable(fileKind);
        var finding = await context.StorageReconciliationFindings.SingleOrDefaultAsync(
            item => item.FileKind == fileKind
            && item.FileName == fileName && item.Kind == kind, cancellationToken);
        if (finding == null && !present)
        {
            return;
        }

        if (finding == null)
        {
            finding = new StorageReconciliationFinding
            {
                FileKind = fileKind,
                FileName = fileName,
                Kind = kind,
                FirstSeenAt = clock.GetUtcNow()
            };
            context.StorageReconciliationFindings.Add(finding);
        }

        if (present && finding.ResolvedAt.HasValue)
        {
            finding.FirstSeenAt = clock.GetUtcNow();
            finding.CleanupOperationId = null;
        }

        finding.LastSeenAt = clock.GetUtcNow();
        finding.ResolvedAt = present ? null : clock.GetUtcNow();
        files.EnsureAvailable(fileKind);
        await context.SaveChangesAsync(cancellationToken);
        context.Entry(finding).State = EntityState.Detached;
    }
}
