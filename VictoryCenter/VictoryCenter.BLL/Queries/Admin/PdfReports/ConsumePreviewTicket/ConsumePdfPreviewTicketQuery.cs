using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.ConsumePreviewTicket;

public class ConsumePdfPreviewTicketQuery : IRequest<Result<PdfReportFileDto>>
{
    public ConsumePdfPreviewTicketQuery(string ticket)
    {
        Ticket = ticket;
    }

    public string Ticket { get; set; }
}
