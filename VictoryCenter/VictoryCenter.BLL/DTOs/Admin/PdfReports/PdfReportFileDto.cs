namespace VictoryCenter.BLL.DTOs.Admin.PdfReports;

public class PdfReportFileDto
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
}
