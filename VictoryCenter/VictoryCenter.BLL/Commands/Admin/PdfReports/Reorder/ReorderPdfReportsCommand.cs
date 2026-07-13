using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Reorder;

public record ReorderPdfReportsCommand(ReorderPdfReportsDto ReorderPdfReportsDto)
    : IRequest<Result<Unit>>;
