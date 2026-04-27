using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PdfSection.Get;

public record GetPdfSectionLocalizationsQuery : IRequest<Result<List<PdfSectionLocalizationDto>>>;
