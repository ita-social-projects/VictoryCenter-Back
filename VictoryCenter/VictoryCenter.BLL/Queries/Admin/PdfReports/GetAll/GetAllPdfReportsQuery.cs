using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;

public record GetAllPdfReportsQuery(BaseFilterDto FilterDto)
    : IRequest<Result<PaginationResult<PdfReportDto>>>;
