using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Update;

public record UpdatePdfReportCommand(long Id, string Name)
    : IRequest<Result<PdfReportDto>>;
