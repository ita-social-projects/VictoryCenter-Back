using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;

namespace VictoryCenter.BLL.Queries.Admin.PdfSectionWithReport;

public record GetPdfSectionWithReportsQuery : IRequest<Result<PdfSectionWithReportsDto>>;
