using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Public.PdfReports.GetByLanguageId;

public record GetPublicPdfReportsByLanguageIdQuery(long LanguageId, BaseFilterDto Filter)
    : IRequest<Result<PaginationResult<PdfReportDto>>>;
