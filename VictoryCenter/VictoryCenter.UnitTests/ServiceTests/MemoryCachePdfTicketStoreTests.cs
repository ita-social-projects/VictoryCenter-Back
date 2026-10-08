using Microsoft.Extensions.Caching.Memory;
using VictoryCenter.BLL.Services.PdfReports;

namespace VictoryCenter.UnitTests.ServiceTests;

public class MemoryCachePdfTicketStoreTests : IDisposable
{
    private readonly MemoryCache _memoryCache;
    private readonly MemoryCachePdfTicketStore _ticketStore;

    public MemoryCachePdfTicketStoreTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _ticketStore = new MemoryCachePdfTicketStore(_memoryCache);
    }

    [Fact]
    public void TryGetTicket_WhenTicketExists_ShouldReturnTrueAndPdfId()
    {
        // Arrange
        var ticketId = "test-ticket-123";
        long expectedPdfId = 42;
        var cacheKey = $"PdfTicket_{ticketId}";

        _memoryCache.Set(cacheKey, expectedPdfId);

        // Act
        var isSuccess = _ticketStore.TryGetTicket(ticketId, out long actualPdfId);

        // Assert
        Assert.True(isSuccess);
        Assert.Equal(expectedPdfId, actualPdfId);

        var stillExists = _memoryCache.TryGetValue(cacheKey, out _);
        Assert.True(stillExists);
    }

    [Fact]
    public void TryGetTicket_WhenTicketDoesNotExist_ShouldReturnFalseAndDefaultPdfId()
    {
        // Arrange
        var ticketId = "non-existent-ticket";

        // Act
        var isSuccess = _ticketStore.TryGetTicket(ticketId, out long actualPdfId);

        // Assert
        Assert.False(isSuccess);
        Assert.Equal(default, actualPdfId);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }
}
