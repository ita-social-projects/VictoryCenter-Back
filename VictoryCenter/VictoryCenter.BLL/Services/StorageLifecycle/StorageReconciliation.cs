using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Helpers;
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
        HashSet<Guid> skippedFindings = [];
        foreach (var fileKind in Enum.GetValues<StorageFileKind>())
        {
            files.EnsureAvailable(fileKind);
        }

        foreach (var fileKind in Enum.GetValues<StorageFileKind>())
        {
            var (referenceFindings, referenceFailures) = await ScanReferencesAsync(fileKind, skippedFindings, cancellationToken);
            var (inventoryFindings, inventoryFailures) = await ScanFilesAsync(fileKind, skippedFindings, cancellationToken);
            findings += referenceFindings + inventoryFindings;
            failures += referenceFailures + inventoryFailures;
        }

        failures += await RecheckStaleFindingsAsync(scanStartedAt, skippedFindings, cancellationToken);
        if (failures > 0)
        {
            logger.LogWarning("Storage reconciliation skipped {FailureCount} entries because of storage errors", failures);
        }

        return findings;
    }

    private async Task<(int Findings, int Failures)> ScanReferencesAsync(
        StorageFileKind fileKind, HashSet<Guid> skippedFindings, CancellationToken cancellationToken)
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
            List<Observation> observations = [];
            foreach (var reference in references)
            {
                try
                {
                    var name = fileKind == StorageFileKind.Image
                        ? StorageFileNames.Image(reference.Name, reference.MimeType) : reference.Name;
                    var missing = !files.Exists(fileKind, name);
                    observations.Add(new Observation(name, missing));
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

            failures += await RecordBatchAsync(fileKind, StorageFindingKind.Missing, observations, skippedFindings, cancellationToken);
            afterId = references.Count > 0 ? references[^1].Id : afterId;
        }
        while (references.Count == ScanBatchSize);

        return (findings, failures);
    }

    private async Task<(int Findings, int Failures)> ScanFilesAsync(
        StorageFileKind fileKind, HashSet<Guid> skippedFindings, CancellationToken cancellationToken)
    {
        var findings = 0;
        var failures = 0;
        foreach (var batch in files.Enumerate(fileKind).Chunk(ScanBatchSize))
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<string> names = [];
            foreach (var fileName in batch)
            {
                try
                {
                    LocalStorageFiles.ValidateFileName(fileName);
                    names.Add(fileName);
                }
                catch (ArgumentException exception)
                {
                    failures++;
                    LogSkippedFile(fileKind, fileName, exception);
                }
            }

            if (names.Count == 0)
            {
                continue;
            }

            var fileNames = names.ToArray();
            var referenced = (await StorageReferences.ReferencedNames(context, fileKind, fileNames).ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var protectedNames = await StorageReferences.ProtectedNamesAsync(context, fileKind, fileNames, cancellationToken);
            List<Observation> observations = [];
            foreach (var fileName in names)
            {
                var unreferenced = !referenced.Contains(fileName);

                // Pending cleanup must not resolve its own finding before execution.
                if (unreferenced && protectedNames.Contains(fileName))
                {
                    continue;
                }

                observations.Add(new Observation(fileName, unreferenced));
                findings += unreferenced ? 1 : 0;
            }

            failures += await RecordBatchAsync(fileKind, StorageFindingKind.Unreferenced, observations, skippedFindings, cancellationToken);
        }

        return (findings, failures);
    }

    private async Task<int> RecheckStaleFindingsAsync(
        DateTimeOffset scanStartedAt, HashSet<Guid> skippedFindings, CancellationToken cancellationToken)
    {
        var failures = 0;
        List<StorageReconciliationFinding> staleFindings;
        do
        {
            staleFindings = await context.StorageReconciliationFindings.AsNoTracking()
                .Where(finding => finding.ResolvedAt == null && finding.LastSeenAt < scanStartedAt && !skippedFindings.Contains(finding.Id))
                .OrderBy(finding => finding.LastSeenAt).ThenBy(finding => finding.Id)
                .Take(ScanBatchSize).ToListAsync(cancellationToken);
            foreach (var group in staleFindings.GroupBy(finding => new { finding.FileKind, finding.Kind }))
            {
                var fileNames = group.Select(finding => finding.FileName).ToArray();
                var referenced = (await StorageReferences.ReferencedNames(context, group.Key.FileKind, fileNames).ToListAsync(cancellationToken))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                List<Observation> observations = [];
                foreach (var finding in group)
                {
                    try
                    {
                        var exists = files.Exists(finding.FileKind, finding.FileName);
                        var hasReference = referenced.Contains(finding.FileName);
                        var stillPresent = finding.Kind == StorageFindingKind.Missing ? hasReference && !exists : !hasReference && exists;
                        observations.Add(new Observation(finding.FileName, stillPresent));
                    }
                    catch (Exception exception) when (IsEntryError(exception))
                    {
                        skippedFindings.Add(finding.Id);
                        failures++;
                        logger.LogWarning("Storage reconciliation skipped finding {FindingId} with {ErrorCode}", finding.Id, exception.GetType().Name);
                    }
                }

                failures += await RecordBatchAsync(group.Key.FileKind, group.Key.Kind, observations, skippedFindings, cancellationToken);
            }
        }
        while (staleFindings.Count == ScanBatchSize);

        return failures;
    }

    private static bool IsEntryError(Exception exception)
    {
        if (exception is DirectoryNotFoundException)
        {
            return false;
        }

        return exception is IOException or UnauthorizedAccessException or ArgumentException;
    }

    private async Task<int> RecordBatchAsync(
        StorageFileKind fileKind,
        StorageFindingKind kind,
        IReadOnlyCollection<Observation> observations,
        HashSet<Guid> skippedFindings,
        CancellationToken cancellationToken)
    {
        if (observations.Count == 0)
        {
            return 0;
        }

        try
        {
            await SaveBatchAsync(fileKind, kind, observations, cancellationToken);
            return 0;
        }
        catch (DbUpdateException exception) when (IsFindingConflict(exception))
        {
            // A competing scan or operator changed this batch. Reload each entry so
            // healthy findings can progress without overwriting a new cleanup link.
        }

        var failures = 0;
        foreach (var observation in observations)
        {
            try
            {
                await SaveBatchAsync(fileKind, kind, [observation], cancellationToken);
            }
            catch (DbUpdateException exception) when (IsFindingConflict(exception))
            {
                failures++;
                foreach (var entry in exception.Entries)
                {
                    skippedFindings.Add(((StorageReconciliationFinding)entry.Entity).Id);
                }

                LogSkippedFile(fileKind, observation.FileName, exception);
            }
        }

        return failures;
    }

    private static bool IsFindingConflict(DbUpdateException exception) =>
        exception.Entries.Count > 0 && exception.Entries.All(entry => entry.Entity is StorageReconciliationFinding)
        && (exception is DbUpdateConcurrencyException || exception.IsUniqueConstraintException());

    private async Task SaveBatchAsync(
        StorageFileKind fileKind, StorageFindingKind kind, IReadOnlyCollection<Observation> observations, CancellationToken cancellationToken)
    {
        files.EnsureAvailable(fileKind);
        var names = observations.Select(observation => observation.FileName).Distinct().ToArray();
        var existing = await context.StorageReconciliationFindings
            .Where(finding => finding.FileKind == fileKind && finding.Kind == kind && names.Contains(finding.FileName))
            .ToListAsync(cancellationToken);
        var findings = existing.ToDictionary(finding => finding.FileName, StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var observation in observations)
            {
                if (!findings.TryGetValue(observation.FileName, out var finding))
                {
                    if (!observation.Present)
                    {
                        continue;
                    }

                    finding = new StorageReconciliationFinding
                    {
                        FileKind = fileKind, FileName = observation.FileName, Kind = kind, FirstSeenAt = clock.GetUtcNow()
                    };
                    findings.Add(observation.FileName, finding);
                    context.StorageReconciliationFindings.Add(finding);
                }

                if (observation.Present && finding.ResolvedAt.HasValue)
                {
                    finding.FirstSeenAt = clock.GetUtcNow();
                    finding.CleanupOperationId = null;
                }

                finding.LastSeenAt = clock.GetUtcNow();
                finding.ResolvedAt = observation.Present ? null : clock.GetUtcNow();
            }

            files.EnsureAvailable(fileKind);
            if (findings.Count > 0)
            {
                await context.SaveChangesAsync(cancellationToken);
            }
        }
        finally
        {
            foreach (var finding in findings.Values)
            {
                context.Entry(finding).State = EntityState.Detached;
            }
        }
    }

    private void LogSkippedFile(StorageFileKind fileKind, string fileName, Exception exception)
    {
        var fileNameHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(fileName)));
        logger.LogWarning(
            "Storage reconciliation skipped file {FileNameHash} of {FileKind} with {ErrorCode}",
            fileNameHash,
            fileKind,
            exception.GetType().Name);
    }

    private readonly record struct Observation(string FileName, bool Present);
}
