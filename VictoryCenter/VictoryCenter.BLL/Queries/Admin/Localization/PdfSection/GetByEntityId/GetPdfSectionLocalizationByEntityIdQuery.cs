using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PdfSection.GetByEntityId;

public record GetPdfSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<PdfSectionLocalizationDto>>>;
