using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;

namespace VictoryCenter.BLL.Queries.Admin.PdfSection.Get;

public record GetPdfSectionQuery : IRequest<Result<PdfSectionDto>>;
