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
        return context.StorageOperations.AnyAsync(
            operation => operation.FileKind == kind
                && (!excludedOperationId.HasValue || operation.Id != excludedOperationId.Value)
                && (operation.FileName == name || (temporaryOwner != null
                    && operation.Kind == StorageOperationKind.UploadCleanup
                    && (operation.FileName == temporaryOwner || operation.FileName.StartsWith(legacyPrefix!))))
                && (operation.Status == StorageOperationStatus.Pending || operation.Status == StorageOperationStatus.Processing), cancellationToken);
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
                    MimeType = string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Jpg)
                        ? ImageMimeTypes.Jpeg : image.MimeType.ToLower()
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
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return context.Images.AnyAsync(
            image => image.BlobName == name
            && ((string.Equals(extension, FileExtensions.JpgWithDot)
                    && (string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Jpeg)
                        || string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Jpg)))
                || (string.Equals(extension, FileExtensions.PngWithDot) && string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Png))
                || (string.Equals(extension, FileExtensions.WebpWithDot) && string.Equals(image.MimeType.ToLower(), ImageMimeTypes.Webp))), cancellationToken);
    }
}

public sealed class StorageReference
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string MimeType { get; init; } = null!;
}
