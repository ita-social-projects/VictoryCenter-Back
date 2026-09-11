using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using VictoryCenter.BLL.Commands.Admin.PdfReports.GenerateTicket;

public class GeneratePdfPreviewTicketHandler : IRequestHandler<GeneratePdfPreviewTicketCommand, Result<string>>
{
    private readonly IMemoryCache _cache;

    public GeneratePdfPreviewTicketHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<Result<string>> Handle(GeneratePdfPreviewTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketId = Guid.NewGuid().ToString();

        _cache.Set($"PdfTicket_{ticketId}", request.Id, TimeSpan.FromSeconds(30));

        return Result.Ok(ticketId);
    }
}
