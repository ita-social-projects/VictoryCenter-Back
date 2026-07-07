namespace VictoryCenter.BLL.DTOs.Admin.PdfReports;

public record ReorderPdfReportsDto
{
    public long LanguageId { get; init; }
    public List<long> OrderedIds { get; init; } = [];
}
