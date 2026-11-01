using Microsoft.AspNetCore.Http;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class PdfStorageService(PdfService storage, StorageUploadReservations reservations) : IPdfService
{
    public async Task<string> UploadPdfAsync(IFormFile file, string? fileName = null)
    {
        var name = fileName ?? Guid.NewGuid().ToString("N");
        if (name.Contains('/') || name.Contains('\\'))
        {
            throw new ArgumentException("Invalid PDF storage identifier.", nameof(fileName));
        }

        name = Path.GetFileNameWithoutExtension(name);
        var finalName = name + FileExtensions.PdfWithDot;
        await reservations.ReserveAsync(StorageFileKind.Pdf, finalName);
        return await storage.UploadPdfAsync(file, finalName);
    }

    public Task<MemoryStream> GetPdfAsync(string fileName) => storage.GetPdfAsync(fileName);

    public void DeletePdf(string fileName)
        => reservations.ScheduleDeletion(StorageFileKind.Pdf, fileName);
}
