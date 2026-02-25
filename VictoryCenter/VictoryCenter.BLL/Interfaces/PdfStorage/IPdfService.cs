using Microsoft.AspNetCore.Http;

namespace VictoryCenter.BLL.Interfaces.PdfStorage;

public interface IPdfService
{
    Task<string> UploadPdfAsync(IFormFile file, string? fileName = null);
    Task<MemoryStream> GetPdfAsync(string fileName);
    void DeletePdf(string fileName);
}
