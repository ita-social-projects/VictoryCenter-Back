using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.DTOs.Admin.PdfSection;

public class PdfSectionWithReportsDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<PdfReportDto> PdfFiles { get; set; } = [];
}
