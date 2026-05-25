using Microsoft.AspNetCore.Http;

namespace VictoryCenter.BLL.DTOs.Admin.PdfReports;

public record CreatePdfReportDto
{
    public required IFormFile File { get; init; }
    public long LanguageId { get; init; }
}
