using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PdfSections.Update;

public record UpdatePdfSectionLocalizationCommand(
    UpdatePdfSectionLocalizationDto UpdatePdfSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<PdfSectionLocalizationDto>>;
