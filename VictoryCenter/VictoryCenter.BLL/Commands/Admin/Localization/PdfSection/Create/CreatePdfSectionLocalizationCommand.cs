using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PdfSections.Create;

public record CreatePdfSectionLocalizationCommand(CreatePdfSectionLocalizationDto Dto)
    : IRequest<Result<PdfSectionLocalizationDto>>;
