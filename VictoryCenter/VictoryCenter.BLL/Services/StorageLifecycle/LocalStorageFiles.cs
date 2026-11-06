using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class LocalStorageFiles(
    IOptions<BlobEnvironmentVariables> images,
    IOptions<PdfEnvironmentVariables> pdfs)
{
    private readonly string _imageRoot = GetExistingRoot(images.Value.FullPath);
    private readonly string _pdfRoot = GetExistingRoot(pdfs.Value.FullPath);

    public void EnsureAvailable(StorageFileKind fileKind) => GetExistingRoot(GetRoot(fileKind));

    public string GetPath(StorageFileKind fileKind, string fileName)
    {
        ValidateFileName(fileName);
        var root = GetExistingRoot(GetRoot(fileKind));
        var path = Path.Combine(root, fileName);
        if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            throw new IOException("Symbolic links are not managed storage files.");
        }

        return path;
    }

    public bool Exists(StorageFileKind fileKind, string fileName)
    {
        // Unlike File.Exists, GetAttributes does not hide access or I/O errors.
        try
        {
            var attributes = File.GetAttributes(GetPath(fileKind, fileName));
            if ((attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0)
            {
                throw new IOException("The storage entry is not a regular file.");
            }

            return true;
        }
        catch (FileNotFoundException)
        {
            EnsureAvailable(fileKind);
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            EnsureAvailable(fileKind);
            return false;
        }
    }

    public IEnumerable<string> Enumerate(StorageFileKind fileKind)
    {
        var root = GetExistingRoot(GetRoot(fileKind));

        return Directory.EnumerateFiles(root, "*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName).OfType<string>().Where(name => IsManagedFile(fileKind, name));
    }

    internal static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || fileName.Length > StorageFileNames.MaxLength || fileName is "." or "..")
        {
            throw new ArgumentException("Invalid storage file name.", nameof(fileName));
        }

        if (fileName.Contains('/') || fileName.Contains('\\') || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("Invalid storage file name.", nameof(fileName));
        }
    }

    private static string GetExistingRoot(string root)
    {
        try
        {
            if ((File.GetAttributes(root) & FileAttributes.Directory) == 0)
            {
                throw new IOException("The storage root is not a directory.");
            }
        }
        catch (FileNotFoundException exception)
        {
            throw new DirectoryNotFoundException("The storage root is unavailable.", exception);
        }

        return Path.GetFullPath(root);
    }

    private static bool IsManagedFile(StorageFileKind fileKind, string fileName)
    {
        if (fileName.StartsWith('.') && fileName.EndsWith(FileExtensions.TemporaryWithDot, StringComparison.OrdinalIgnoreCase))
        {
            return StorageFileNames.TemporaryOwner(fileName) != null;
        }

        var extension = Path.GetExtension(fileName);
        return fileKind == StorageFileKind.Pdf
            ? string.Equals(extension, FileExtensions.PdfWithDot, StringComparison.OrdinalIgnoreCase)
            : string.Equals(extension, FileExtensions.JpgWithDot, StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, FileExtensions.PngWithDot, StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, FileExtensions.WebpWithDot, StringComparison.OrdinalIgnoreCase);
    }

    private string GetRoot(StorageFileKind fileKind) => fileKind switch
    {
        StorageFileKind.Image => _imageRoot,
        StorageFileKind.Pdf => _pdfRoot,
        _ => throw new ArgumentOutOfRangeException(nameof(fileKind))
    };
}
