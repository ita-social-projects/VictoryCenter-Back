using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;

public class GetPdfReportPreviewByIdQuery : IRequest<Result<PdfReportFileDto>>
{
    public GetPdfReportPreviewByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
