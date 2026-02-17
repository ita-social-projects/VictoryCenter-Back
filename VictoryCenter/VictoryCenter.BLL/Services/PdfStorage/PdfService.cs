using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;

namespace VictoryCenter.BLL.Services.PdfStorage;

public class PdfService : IPdfService
{
    private const string PdfExtension = "pdf";
    private const string PdfMimeType = "application/pdf";
    private readonly PdfEnvironmentVariables _pdfEnv;
    private static readonly byte[] PdfSignature = { 0x25, 0x50, 0x44, 0x46 };

    public PdfService(IOptions<PdfEnvironmentVariables> environment, IHttpContextAccessor httpContextAccessor)
    {
        _pdfEnv = environment.Value;

        try
        {
            Directory.CreateDirectory(_pdfEnv.FullPath);
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(_pdfEnv.FullPath, "Failed to create PDF directory", ex);
        }
    }

    public async Task<string> UploadPdfAsync(IFormFile file, string? fileName = null)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidPdfFormatException("File is empty or null");
        }

        if (!file.ContentType.Equals(PdfMimeType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidPdfFormatException($"Invalid file format. Expected PDF, got {file.ContentType}");
        }

        await ValidatePdfSignatureAsync(file);

        var targetFileName = string.IsNullOrWhiteSpace(fileName)
            ? Path.GetFileNameWithoutExtension(file.FileName)
            : fileName;

        ValidateFileName(targetFileName);

        var fileNameWithExtension = $"{targetFileName}.{PdfExtension}";
        var filePath = Path.Combine(_pdfEnv.FullPath, fileNameWithExtension);

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(fileStream);

            return fileNameWithExtension;
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(fileNameWithExtension, "Failed to save PDF file", ex);
        }
    }

    public async Task<MemoryStream> GetPdfAsync(string fileName)
    {
        var normalizedFileName = NormalizeFileName(fileName);
        ValidateFileName(normalizedFileName);

        var filePath = Path.Combine(_pdfEnv.FullPath, normalizedFileName);

        if (!File.Exists(filePath))
        {
            throw new BlobNotFoundException(normalizedFileName, $"PDF file not found: {filePath}");
        }

        try
        {
            var bytes = await File.ReadAllBytesAsync(filePath);
            return new MemoryStream(bytes);
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobFileSystemException(normalizedFileName, "Failed to read PDF file", ex);
        }
    }

    private static async Task ValidatePdfSignatureAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var buffer = new byte[4];
        var bytesRead = await stream.ReadAsync(buffer, 0, 4);

        if (bytesRead < 4 || !buffer.SequenceEqual(PdfSignature))
        {
            throw new InvalidPdfFormatException("File does not have a valid PDF signature");
        }

        stream.Position = 0;
    }

    private static string NormalizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BlobFileNameException(fileName, "File name cannot be empty");
        }

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        return $"{nameWithoutExtension}.{PdfExtension}";
    }

    private static void ValidateFileName(string name)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(name);

        if (string.IsNullOrWhiteSpace(nameWithoutExtension)
            || nameWithoutExtension.Contains("..")
            || name.Contains('/')
            || name.Contains('\\')
            || Path.GetInvalidFileNameChars().Any(nameWithoutExtension.Contains))
        {
            throw new BlobFileNameException(name, $"Invalid file name: {name}");
        }
    }
}
