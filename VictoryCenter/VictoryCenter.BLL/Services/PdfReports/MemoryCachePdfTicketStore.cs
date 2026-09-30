using Microsoft.Extensions.Caching.Memory;
using VictoryCenter.BLL.Interfaces.PdfReports;

namespace VictoryCenter.BLL.Services.PdfReports;

public class MemoryCachePdfTicketStore : IPdfTicketStore
{
    private readonly IMemoryCache _cache;

    private static readonly object _lock = new();

    public MemoryCachePdfTicketStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetTicket(string ticketId, out long pdfId)
    {
        var key = $"PdfTicket_{ticketId}";

        return _cache.TryGetValue(key, out pdfId);
    }
}
