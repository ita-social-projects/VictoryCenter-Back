using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetById;

public record GetPdfReportByIdQuery(long Id)
    : IRequest<Result<Stream>>;
