using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewTicket;

public class GetPdfPreviewTicketQuery : IRequest<Result<PdfReportFileDto>>
{
    public GetPdfPreviewTicketQuery(string ticket)
    {
        Ticket = ticket;
    }

    public string Ticket { get; set; }
}
