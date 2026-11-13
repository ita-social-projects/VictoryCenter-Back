using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using VictoryCenter.BLL.Interfaces.Audit;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private static readonly ConcurrentDictionary<Type, string[]> AuditedPropertiesByType = new();

    private readonly IAuditLogFactory _auditLogFactory;
    private readonly ConditionalWeakTable<DbContext, PendingCreates> _pendingCreates = new();

    public AuditInterceptor(IAuditLogFactory auditLogFactory)
    {
        _auditLogFactory = auditLogFactory;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is { } context)
        {
            AddAuditLogs(context);
            TrackCreates(context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            await AddAuditLogsAsync(context, cancellationToken);
            await TrackCreatesAsync(context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context && _pendingCreates.TryGetValue(context, out var pending))
        {
            _pendingCreates.Remove(context);
            try
            {
                foreach (var entry in pending.Entries)
                {
                    context.Set<AuditLog>().Add(CreateAuditLog(entry, AuditOperation.Create));
                }

                await context.SaveChangesAsync(cancellationToken);

                if (pending.OwnTransaction is not null)
                {
                    await pending.OwnTransaction.CommitAsync(cancellationToken);
                }
            }
            finally
            {
                if (pending.OwnTransaction is not null)
                {
                    await pending.OwnTransaction.DisposeAsync();
                }
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is { } context && _pendingCreates.TryGetValue(context, out var pending))
        {
            _pendingCreates.Remove(context);
            try
            {
                foreach (var entry in pending.Entries)
                {
                    context.Set<AuditLog>().Add(CreateAuditLog(entry, AuditOperation.Create));
                }

                context.SaveChanges();

                pending.OwnTransaction?.Commit();
            }
            finally
            {
                pending.OwnTransaction?.Dispose();
            }
        }

        return base.SavedChanges(eventData, result);
    }

    public override async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context && _pendingCreates.TryGetValue(context, out var pending))
        {
            _pendingCreates.Remove(context);
            if (pending.OwnTransaction is not null)
            {
                await pending.OwnTransaction.DisposeAsync();
            }
        }

        await base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Context is { } context && _pendingCreates.TryGetValue(context, out var pending))
        {
            _pendingCreates.Remove(context);
            pending.OwnTransaction?.Dispose();
        }

        base.SaveChangesFailed(eventData);
    }

    private static bool IsAuditable(EntityEntry entry) =>
        entry.Metadata.ClrType.IsDefined(typeof(AuditableAttribute), inherit: true);

    private static string[] GetAuditedProperties(EntityEntry entry)
    {
        return AuditedPropertiesByType.GetOrAdd(entry.Metadata.ClrType, _ => entry.Metadata
            .GetProperties()
            .Where(p => p.PropertyInfo?.IsDefined(typeof(AuditedAttribute), inherit: true) == true)
            .Select(p => p.Name)
            .ToArray());
    }

    private static string GetEntityId(EntityEntry entry)
    {
        var keys = entry.Metadata.FindPrimaryKey()!.Properties;
        return keys.Count == 1
            ? entry.Property(keys[0].Name).CurrentValue?.ToString() ?? string.Empty
            : string.Join(";", keys.Select(k => $"{k.Name}={entry.Property(k.Name).CurrentValue}"));
    }

    private static Dictionary<string, AuditChange> BuildChanges(EntityEntry entry, AuditOperation operation, PropertyValues? databaseValues)
    {
        var changes = new Dictionary<string, AuditChange>();
        var auditedProperties = GetAuditedProperties(entry);

        foreach (var propertyName in auditedProperties)
        {
            var property = entry.Property(propertyName);
            if (operation == AuditOperation.Create)
            {
                if (property.CurrentValue is not null)
                {
                    changes[propertyName] = new AuditChange(null, property.CurrentValue);
                }

                continue;
            }

            if (operation == AuditOperation.Delete)
            {
                if (property.OriginalValue is not null)
                {
                    changes[propertyName] = new AuditChange(property.OriginalValue, null);
                }

                continue;
            }

            var oldValue = databaseValues is not null ? databaseValues[propertyName] : property.OriginalValue;
            if (Equals(oldValue, property.CurrentValue))
            {
                continue;
            }

            changes[propertyName] = new AuditChange(oldValue, property.CurrentValue);
        }

        return changes;
    }

    private static List<EntityEntry> GetUpdatedOrDeletedEntries(DbContext context)
    {
        var entries = new List<EntityEntry>();
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog)
            {
                if (entry.State is EntityState.Modified or EntityState.Deleted)
                {
                    throw new InvalidOperationException("Audit log entries cannot be modified or deleted.");
                }

                continue;
            }

            if (IsAuditable(entry) && entry.State is EntityState.Modified or EntityState.Deleted)
            {
                entries.Add(entry);
            }
        }

        return entries;
    }

    private static bool HasActualChanges(EntityEntry entry, PropertyValues databaseValues) =>
        entry.Properties.Any(p => !p.Metadata.IsConcurrencyToken
        && !Equals(databaseValues[p.Metadata.Name], p.CurrentValue));

    private void AddAuditLogs(DbContext context)
    {
        foreach (var entry in GetUpdatedOrDeletedEntries(context))
        {
            var databaseValues = entry.State == EntityState.Modified ? entry.GetDatabaseValues() : null;
            AddAuditLog(context, entry, databaseValues);
        }
    }

    private async Task AddAuditLogsAsync(DbContext context, CancellationToken cancellationToken)
    {
        foreach (var entry in GetUpdatedOrDeletedEntries(context))
        {
            var databaseValues = entry.State == EntityState.Modified
                ? await entry.GetDatabaseValuesAsync(cancellationToken)
                : null;
            AddAuditLog(context, entry, databaseValues);
        }
    }

    private void AddAuditLog(DbContext context, EntityEntry entry, PropertyValues? databaseValues)
    {
        if (databaseValues is not null && !HasActualChanges(entry, databaseValues))
        {
            return;
        }

        var operation = entry.State == EntityState.Modified ? AuditOperation.Update : AuditOperation.Delete;
        context.Set<AuditLog>().Add(CreateAuditLog(entry, operation, databaseValues));
    }

    private AuditLog CreateAuditLog(
        EntityEntry entry,
        AuditOperation operation,
        PropertyValues? databaseValues = null) =>
        _auditLogFactory.Create(
            entry.Metadata.ClrType.Name,
            GetEntityId(entry),
            operation,
            BuildChanges(entry, operation, databaseValues));

    private async Task TrackCreatesAsync(DbContext context, CancellationToken cancellationToken)
    {
        var created = context.ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added && IsAuditable(entry))
            .ToList();
        if (created.Count == 0)
        {
            return;
        }

        IDbContextTransaction? ownTransaction = null;
        if (context.Database.CurrentTransaction is null && Transaction.Current is null)
        {
            ownTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }

        _pendingCreates.AddOrUpdate(context, new PendingCreates(created, ownTransaction));
    }

    private void TrackCreates(DbContext context)
    {
        var created = context.ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added && IsAuditable(entry))
            .ToList();
        if (created.Count == 0)
        {
            return;
        }

        IDbContextTransaction? ownTransaction = null;
        if (context.Database.CurrentTransaction is null && Transaction.Current is null)
        {
            ownTransaction = context.Database.BeginTransaction();
        }

        _pendingCreates.AddOrUpdate(context, new PendingCreates(created, ownTransaction));
    }

    private sealed record PendingCreates(List<EntityEntry> Entries, IDbContextTransaction? OwnTransaction);
}
