using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetByLanguage;

public record GetPdfReportsByLanguageQuery(long LanguageId)
    : IRequest<Result<List<PdfReportDto>>>;
