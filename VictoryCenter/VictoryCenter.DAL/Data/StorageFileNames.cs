using VictoryCenter.DAL.Constants;

namespace VictoryCenter.DAL.Data;

public static class StorageFileNames
{
    public const int MaxLength = 150;

    public static string? TemporaryOwner(string fileName)
    {
        // Temporary files follow .{owner}.{guid:N}.tmp, including owners containing dots.
        if (fileName.Length <= FileExtensions.TemporaryWithDot.Length + 1 || !fileName.StartsWith('.')
            || !fileName.EndsWith(FileExtensions.TemporaryWithDot, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var separator = fileName.LastIndexOf('.', fileName.Length - FileExtensions.TemporaryWithDot.Length - 1);
        return separator > 1 && Guid.TryParseExact(fileName.AsSpan(separator + 1, fileName.Length - FileExtensions.TemporaryWithDot.Length - separator - 1), "N", out _)
            ? fileName[1..separator] : null;
    }

    public static string Image(string name, string mimeType)
    {
        var extension = mimeType.ToLowerInvariant() switch
        {
            FileMimeTypes.ImageJpeg or FileMimeTypes.ImageJpg => FileExtensions.Jpg,
            FileMimeTypes.ImagePng => FileExtensions.Png,
            FileMimeTypes.ImageWebp => FileExtensions.Webp,
            _ => throw new ArgumentException("Unsupported image MIME type.", nameof(mimeType))
        };

        return $"{name}.{extension}";
    }
}
