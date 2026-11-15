using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class StorageUploadReservations(
    IServiceScopeFactory scopeFactory,
    LocalStorageFiles files,
    IOptions<StorageLifecycleOptions> options,
    TimeProvider clock)
{
    public void ScheduleDeletion(StorageFileKind fileKind, string fileName)
    {
        files.GetPath(fileKind, fileName);
        using var suppression = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        context.StorageOperations.Add(new StorageOperation
        {
            FileKind = fileKind,
            FileName = fileName,
            Kind = StorageOperationKind.Delete,
            NextAttemptAt = clock.GetUtcNow()
        });
        context.SaveChanges();
        suppression.Complete();
    }

    public async Task ReserveAsync(StorageFileKind fileKind, string fileName)
    {
        if (files.Exists(fileKind, fileName))
        {
            throw new IOException("A storage upload must use a new file identifier.");
        }

        // The intent must survive a rollback of the request's metadata transaction.
        using var suppression = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        if (await context.StorageOperations.AnyAsync(operation => operation.FileKind == fileKind
            && operation.FileName == fileName && operation.Kind == StorageOperationKind.UploadCleanup))
        {
            throw new IOException("The storage identifier has already been reserved.");
        }

        context.StorageOperations.Add(new StorageOperation
        {
            FileKind = fileKind,
            FileName = fileName,
            Kind = StorageOperationKind.UploadCleanup,
            NextAttemptAt = clock.GetUtcNow().AddHours(options.Value.UploadReservationHours)
        });
        await context.SaveChangesAsync();
        suppression.Complete();
    }
}
