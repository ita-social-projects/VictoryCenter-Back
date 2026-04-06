using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Delete;

public record DeletePdfReportCommand(long Id) : IRequest<Result<Unit>>;
