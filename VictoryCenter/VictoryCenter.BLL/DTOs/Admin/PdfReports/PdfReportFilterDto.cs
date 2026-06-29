using VictoryCenter.BLL.DTOs.Admin.Common;

namespace VictoryCenter.BLL.DTOs.Admin.PdfReports;

public record PdfReportFilterDto : BaseFilterDto
{
    public long? LanguageId { get; init; }
}
