using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.ConsumePreviewTicket;

public class ConsumePdfPreviewTicketHandler : IRequestHandler<ConsumePdfPreviewTicketQuery, Result<PdfReportFileDto>>
{
    private readonly IMemoryCache _cache;
    private readonly IMediator _mediator;

    public ConsumePdfPreviewTicketHandler(IMemoryCache cache, IMediator mediator)
    {
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<Result<PdfReportFileDto>> Handle(ConsumePdfPreviewTicketQuery request, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue($"PdfTicket_{request.Ticket}", out long pdfId))
        {
            return Result.Fail<PdfReportFileDto>("Invalid or expired preview ticket.");
        }

        _cache.Remove($"PdfTicket_{request.Ticket}");

        return await _mediator.Send(new GetPdfReportPreviewByIdQuery(pdfId), cancellationToken);
    }
}
