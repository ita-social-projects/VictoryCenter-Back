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

    public bool TryConsumeTicket(string ticketId, out long pdfId)
    {
        var key = $"PdfTicket_{ticketId}";

        lock (_lock)
        {
            if (_cache.TryGetValue(key, out pdfId))
            {
                _cache.Remove(key);
                return true;
            }
        }

        pdfId = default;
        return false;
    }
}
