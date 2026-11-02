using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Interfaces.HtmlSanitization;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.BLL.Interceptors;

public class RichTextSanitizationInterceptor : SaveChangesInterceptor
{
    private static readonly ConcurrentDictionary<Type, string[]> RichTextPropertiesByType = new();

    private readonly IRichTextSanitizer _sanitizer;
    private readonly ILogger<RichTextSanitizationInterceptor> _logger;

    public RichTextSanitizationInterceptor(
        IRichTextSanitizer sanitizer,
        ILogger<RichTextSanitizationInterceptor> logger)
    {
        _sanitizer = sanitizer;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SanitizeRichTextProperties(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SanitizeRichTextProperties(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static string[] GetRichTextProperties(EntityEntry entry)
    {
        return RichTextPropertiesByType.GetOrAdd(entry.Metadata.ClrType, _ => entry.Metadata
            .GetProperties()
            .Where(p => p.ClrType == typeof(string)
                        && p.PropertyInfo?.IsDefined(typeof(RichTextAttribute), inherit: true) == true)
            .Select(p => p.Name)
            .ToArray());
    }

    private void SanitizeRichTextProperties(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is not EntityState.Added and not EntityState.Modified)
            {
                continue;
            }

            // Also sanitizes unchanged properties, so content stored before this policy is cleaned on next update
            foreach (var propertyName in GetRichTextProperties(entry))
            {
                var property = entry.Property(propertyName);
                if (property.CurrentValue is not string { Length: > 0 } html)
                {
                    continue;
                }

                var sanitizationResult = _sanitizer.Sanitize(html);
                if (sanitizationResult.Html == html)
                {
                    continue;
                }

                property.CurrentValue = sanitizationResult.Html;

                if (sanitizationResult.HasRemovedContent)
                {
                    LogRemovedContent(entry, propertyName, sanitizationResult.RemovedItems);
                }
            }
        }
    }

    private void LogRemovedContent(EntityEntry entry, string propertyName, IReadOnlyCollection<string> removedItems)
    {
        var entityId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue;

        _logger.LogWarning(
            "Removed disallowed HTML ({RemovedItems}) from {EntityType}.{PropertyName} (Id: {EntityId}, State: {EntityState})",
            string.Join(", ", removedItems),
            entry.Metadata.ClrType.Name,
            propertyName,
            entityId,
            entry.State);
    }
}
