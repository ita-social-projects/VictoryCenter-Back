using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Update;

public record UpdatePdfSectionLocalizationCommand(
    UpdatePdfSectionLocalizationDto UpdatePdfSectionLocalizationDto,
    long LanguageId)
    : IRequest<Result<PdfSectionLocalizationDto>>;
