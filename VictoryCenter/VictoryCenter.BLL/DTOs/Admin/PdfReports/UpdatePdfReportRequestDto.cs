namespace VictoryCenter.BLL.DTOs.Admin.PdfReports;

public record UpdatePdfReportRequestDto
{
    public required string Name { get; init; }
}
