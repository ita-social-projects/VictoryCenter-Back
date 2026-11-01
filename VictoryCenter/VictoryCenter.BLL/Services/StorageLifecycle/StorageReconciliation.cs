using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class StorageReconciliation(
    VictoryCenterDbContext context,
    LocalStorageFiles files,
    TimeProvider clock)
{
    private const int ScanBatchSize = 200;

    public async Task<int> ScanAsync(CancellationToken cancellationToken)
    {
        var scanStartedAt = clock.GetUtcNow();
        var findings = 0;
        foreach (var fileKind in Enum.GetValues<StorageFileKind>())
        {
            long afterId = 0;
            while (true)
            {
                var references = await StorageReferences.Inventory(context, fileKind)
                    .Where(reference => reference.Id > afterId).OrderBy(reference => reference.Id)
                    .Take(ScanBatchSize).ToListAsync(cancellationToken);
                if (references.Count == 0)
                {
                    break;
                }

                foreach (var reference in references)
                {
                    var name = fileKind == StorageFileKind.Image
                        ? StorageFileNames.Image(reference.Name, reference.MimeType) : reference.Name;
                    var missing = !files.Exists(fileKind, name);
                    await RecordAsync(fileKind, name, StorageFindingKind.Missing, missing, cancellationToken);
                    findings += missing ? 1 : 0;
                }

                afterId = references[^1].Id;
            }

            foreach (var fileName in files.Enumerate(fileKind))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var unreferenced = !await StorageReferences.ExistsAsync(context, fileKind, fileName, cancellationToken);
                if (unreferenced && await StorageReferences.IsProtectedAsync(context, fileKind, fileName, cancellationToken))
                {
                    // Pending cleanup must not resolve its own finding before execution.
                    continue;
                }

                await RecordAsync(fileKind, fileName, StorageFindingKind.Unreferenced, unreferenced, cancellationToken);
                findings += unreferenced ? 1 : 0;
            }
        }

        while (true)
        {
            var staleFindings = await context.StorageReconciliationFindings.AsNoTracking()
                .Where(finding => finding.ResolvedAt == null && finding.LastSeenAt < scanStartedAt)
                .OrderBy(finding => finding.LastSeenAt).ThenBy(finding => finding.Id)
                .Take(ScanBatchSize).ToListAsync(cancellationToken);
            if (staleFindings.Count == 0)
            {
                break;
            }

            foreach (var finding in staleFindings)
            {
                var referenced = await StorageReferences.ExistsAsync(context, finding.FileKind, finding.FileName, cancellationToken);
                var exists = files.Exists(finding.FileKind, finding.FileName);
                var stillPresent = finding.Kind == StorageFindingKind.Missing ? referenced && !exists : !referenced && exists;
                await RecordAsync(finding.FileKind, finding.FileName, finding.Kind, stillPresent, cancellationToken);
            }
        }

        return findings;
    }

    private async Task RecordAsync(
        StorageFileKind fileKind,
        string fileName,
        StorageFindingKind kind,
        bool present,
        CancellationToken cancellationToken)
    {
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
        await context.SaveChangesAsync(cancellationToken);
        context.Entry(finding).State = EntityState.Detached;
    }
}
