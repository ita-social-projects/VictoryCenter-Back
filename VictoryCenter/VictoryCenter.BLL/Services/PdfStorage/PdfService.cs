using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;

namespace VictoryCenter.BLL.Services.PdfStorage;

public class PdfService : IPdfService
{
    private readonly PdfEnvironmentVariables _pdfEnv;

    public PdfService(IOptions<PdfEnvironmentVariables> environment, IHttpContextAccessor httpContextAccessor)
    {
        _pdfEnv = environment.Value;

        try
        {
            Directory.CreateDirectory(_pdfEnv.FullPath);
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(_pdfEnv.FullPath, PdfReportConstants.FailedToCreateDirectory, ex);
        }
    }

    public async Task<string> UploadPdfAsync(IFormFile file, string? fileName = null)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidPdfFormatException(PdfReportConstants.FileCannotBeEmpty);
        }

        if (file.Length > PdfReportConstants.MaxPdfSizeInBytes)
        {
            throw new InvalidPdfFormatException(
            string.Format(PdfReportConstants.FileTooLarge, PdfReportConstants.MaxPdfSizeInMb));
        }

        if (file.ContentType is null || !file.ContentType.Equals(PdfReportConstants.PdfMimeType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidPdfFormatException(PdfReportConstants.InvalidPdfFormat);
        }

        await ValidatePdfSignatureAsync(file);

        var targetFileName = string.IsNullOrWhiteSpace(fileName)
            ? Path.GetFileNameWithoutExtension(file.FileName)
            : fileName;

        ValidateFileName(targetFileName);

        var normalizedFileName =
            Path.GetFileNameWithoutExtension(targetFileName);

        var fileNameWithExtension =
            $"{normalizedFileName}.{PdfReportConstants.PdfExtension}";

        var filePath = Path.Combine(_pdfEnv.FullPath, fileNameWithExtension);

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(fileStream);

            return fileNameWithExtension;
        }
        catch (Exception ex)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }

            throw new BlobFileSystemException(fileNameWithExtension, PdfReportConstants.FailedToSavePdf, ex);
        }
    }

    public async Task<MemoryStream> GetPdfAsync(string fileName)
    {
        var normalizedFileName = NormalizeFileName(fileName);
        ValidateFileName(normalizedFileName);

        var filePath = Path.Combine(_pdfEnv.FullPath, normalizedFileName);

        if (!File.Exists(filePath))
        {
            throw new BlobNotFoundException(normalizedFileName, PdfReportConstants.PdfNotFound);
        }

        try
        {
            var bytes = await File.ReadAllBytesAsync(filePath);
            return new MemoryStream(bytes);
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobFileSystemException(normalizedFileName, PdfReportConstants.FailedToReadPdf, ex);
        }
    }

    public void DeletePdf(string fileName)
    {
        var normalizedFileName = NormalizeFileName(fileName);
        ValidateFileName(normalizedFileName);

        var filePath = Path.Combine(_pdfEnv.FullPath, normalizedFileName);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(normalizedFileName, PdfReportConstants.FailedToDeletePdf, ex);
        }
    }

    private static async Task ValidatePdfSignatureAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var buffer = new byte[4];
        var bytesRead = await stream.ReadAsync(buffer, 0, 4);

        if (bytesRead < 4 || !buffer.SequenceEqual(PdfReportConstants.PdfSignature))
        {
            throw new InvalidPdfFormatException(PdfReportConstants.InvalidPdfSignature);
        }

        stream.Position = 0;
    }

    private static string NormalizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BlobFileNameException(fileName, PdfReportConstants.FileNameCannotBeEmpty);
        }

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        return $"{nameWithoutExtension}.{PdfReportConstants.PdfExtension}";
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
            throw new BlobFileNameException(name, PdfReportConstants.InvalidFileName);
        }
    }
}
