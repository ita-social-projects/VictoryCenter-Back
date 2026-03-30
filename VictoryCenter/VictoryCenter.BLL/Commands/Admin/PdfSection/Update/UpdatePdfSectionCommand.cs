using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;

namespace VictoryCenter.BLL.Commands.Admin.PdfSection.Update;

public record UpdatePdfSectionCommand(PdfSectionDto Dto)
    : IRequest<Result<PdfSectionDto>>;
