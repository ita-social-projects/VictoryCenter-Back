using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Interfaces.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.ConsumePreviewTicket;

public class ConsumePdfPreviewTicketHandler : IRequestHandler<ConsumePdfPreviewTicketQuery, Result<PdfReportFileDto>>
{
    private readonly IPdfTicketStore _ticketStore;
    private readonly IMediator _mediator;

    public ConsumePdfPreviewTicketHandler(IPdfTicketStore ticketStore, IMediator mediator)
    {
        _ticketStore = ticketStore;
        _mediator = mediator;
    }

    public async Task<Result<PdfReportFileDto>> Handle(ConsumePdfPreviewTicketQuery request, CancellationToken cancellationToken)
    {
        if (!_ticketStore.TryConsumeTicket(request.Ticket, out long pdfId))
        {
            return Result.Fail<PdfReportFileDto>(PdfReportConstants.InvalidOrExpiredPreviewTicket);
        }

        return await _mediator.Send(new GetPdfReportPreviewByIdQuery(pdfId), cancellationToken);
    }
}
