using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;

public record GetAllPdfReportsQuery(PdfReportFilterDto FilterDto)
    : IRequest<Result<PaginationResult<PdfReportDto>>>;
