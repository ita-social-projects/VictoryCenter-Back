using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.GenerateTicket;

public class GeneratePdfPreviewTicketCommand : IRequest<Result<string>>
{
    public GeneratePdfPreviewTicketCommand(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
