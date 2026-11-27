using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.PdfSection;

namespace VictoryCenter.BLL.Queries.Public.PdfSection.Get;

public record GetPublicPdfSectionQuery : IRequest<Result<PublicPdfSectionDto>>;
