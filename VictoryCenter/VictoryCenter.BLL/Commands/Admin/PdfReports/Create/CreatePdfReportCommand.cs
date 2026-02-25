using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Create;

public record CreatePdfReportCommand(CreatePdfReportDto CreatePdfReportDto) : IRequest<Result<PdfReportDto>>;
