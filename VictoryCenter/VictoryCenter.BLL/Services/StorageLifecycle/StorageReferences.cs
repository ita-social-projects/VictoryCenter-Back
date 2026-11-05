using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public static class StorageReferences
{
    public static Task<bool> IsProtectedAsync(
        VictoryCenterDbContext context,
        StorageFileKind kind,
        string name,
        CancellationToken cancellationToken,
        Guid? excludedOperationId = null)
    {
        var temporaryOwner = StorageFileNames.TemporaryOwner(name);
        var legacyPrefix = temporaryOwner == null ? null : temporaryOwner + ".";
        var operations = context.StorageOperations.Where(operation => operation.FileKind == kind
            && (operation.Status == StorageOperationStatus.Pending || operation.Status == StorageOperationStatus.Processing));
        if (excludedOperationId.HasValue)
        {
            operations = operations.Where(operation => operation.Id != excludedOperationId.Value);
        }

        if (temporaryOwner == null)
        {
            return operations.AnyAsync(operation => operation.FileName == name, cancellationToken);
        }

        return operations.AnyAsync(
            operation => operation.FileName == name || (operation.Kind == StorageOperationKind.UploadCleanup
                && (operation.FileName == temporaryOwner || operation.FileName.StartsWith(legacyPrefix!))), cancellationToken);
    }

    public static IQueryable<StorageReference> Inventory(VictoryCenterDbContext context, StorageFileKind fileKind)
    {
        var references = fileKind switch
        {
            StorageFileKind.Image => context.Images.AsNoTracking()
                .Select(image => new
                {
                    image.Id,
                    Name = image.BlobName,
                    MimeType = string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Png) ? ImageMimeTypes.Png
                        : string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Webp) ? ImageMimeTypes.Webp : ImageMimeTypes.Jpeg
                }),
            StorageFileKind.Pdf => context.PdfReports.AsNoTracking()
                .Select(report => new { report.Id, Name = report.BlobName, MimeType = string.Empty }),
            _ => throw new ArgumentOutOfRangeException(nameof(fileKind))
        };

        // Multiple metadata records may share a file. Inspect each storage reference once.
        return references.GroupBy(reference => new { reference.Name, reference.MimeType })
            .Select(group => new StorageReference
            {
                Id = group.Min(reference => reference.Id),
                Name = group.Key.Name,
                MimeType = group.Key.MimeType
            });
    }

    public static IQueryable<string> ReferencedNames(VictoryCenterDbContext context, StorageFileKind fileKind, string[] fileNames)
    {
        if (fileKind == StorageFileKind.Pdf)
        {
            return context.PdfReports.AsNoTracking().Where(report => fileNames.Contains(report.BlobName))
                .Select(report => report.BlobName).Distinct();
        }

        var blobNames = fileNames.Select(Path.GetFileNameWithoutExtension).ToArray();
        return context.Images.AsNoTracking().Where(image => blobNames.Contains(image.BlobName))
            .Select(image => image.BlobName + (string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Png) ? FileExtensions.PngWithDot
                : string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Webp) ? FileExtensions.WebpWithDot : FileExtensions.JpgWithDot))
            .Where(name => fileNames.Contains(name)).Distinct();
    }

    public static IQueryable<StorageOperation> ProtectingOperations(VictoryCenterDbContext context, StorageFileKind fileKind, string[] fileNames)
    {
        var owners = fileNames.Select(StorageFileNames.TemporaryOwner).OfType<string>().Distinct().ToArray();
        Expression<Func<StorageOperation, bool>> matches = operation => fileNames.Contains(operation.FileName)
            || (operation.Kind == StorageOperationKind.UploadCleanup && owners.Contains(operation.FileName));
        var name = Expression.Property(matches.Parameters[0], nameof(StorageOperation.FileName));
        var upload = Expression.Equal(Expression.Property(matches.Parameters[0], nameof(StorageOperation.Kind)), Expression.Constant(StorageOperationKind.UploadCleanup));

        // Expand legacy owner prefixes into one predicate translated by both EF providers.
        foreach (var owner in owners)
        {
            var startsWith = Expression.Call(name, nameof(string.StartsWith), Type.EmptyTypes, Expression.Constant(owner + "."));
            matches = Expression.Lambda<Func<StorageOperation, bool>>(
                Expression.OrElse(matches.Body, Expression.AndAlso(upload, startsWith)), matches.Parameters);
        }

        return context.StorageOperations.AsNoTracking()
            .Where(operation => operation.FileKind == fileKind
                && (operation.Status == StorageOperationStatus.Pending || operation.Status == StorageOperationStatus.Processing))
            .Where(matches);
    }

    public static async Task<HashSet<string>> ProtectedNamesAsync(
        VictoryCenterDbContext context,
        StorageFileKind fileKind,
        string[] fileNames,
        CancellationToken cancellationToken)
    {
        var operations = await ProtectingOperations(context, fileKind, fileNames)
            .Select(operation => new { operation.FileName, operation.Kind }).Distinct().ToListAsync(cancellationToken);
        return fileNames.Where(name => operations.Any(operation => string.Equals(operation.FileName, name, StringComparison.OrdinalIgnoreCase)
            || (operation.Kind == StorageOperationKind.UploadCleanup && StorageFileNames.TemporaryOwner(name) is { } owner
                && (string.Equals(operation.FileName, owner, StringComparison.OrdinalIgnoreCase)
                    || operation.FileName.StartsWith(owner + ".", StringComparison.OrdinalIgnoreCase)))))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public static Task<bool> ExistsAsync(
        VictoryCenterDbContext context,
        StorageFileKind fileKind,
        string fileName,
        CancellationToken cancellationToken)
    {
        if (fileKind == StorageFileKind.Pdf)
        {
            return context.PdfReports.AnyAsync(report => report.BlobName == fileName, cancellationToken);
        }

        var name = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var images = context.Images.Where(image => image.BlobName == name);
        if (string.Equals(extension, FileExtensions.PngWithDot, StringComparison.OrdinalIgnoreCase))
        {
            return images.AnyAsync(image => string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Png), cancellationToken);
        }

        if (string.Equals(extension, FileExtensions.WebpWithDot, StringComparison.OrdinalIgnoreCase))
        {
            return images.AnyAsync(image => string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Webp), cancellationToken);
        }

        // BlobService stores legacy/unknown MIME types as JPEG. Keep cleanup conservative.
        return string.Equals(extension, FileExtensions.JpgWithDot, StringComparison.OrdinalIgnoreCase)
            ? images.AnyAsync(
                image => !string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Png)
                    && !string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Webp), cancellationToken)
            : Task.FromResult(false);
    }
}

public sealed class StorageReference
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string MimeType { get; init; } = null!;
}
