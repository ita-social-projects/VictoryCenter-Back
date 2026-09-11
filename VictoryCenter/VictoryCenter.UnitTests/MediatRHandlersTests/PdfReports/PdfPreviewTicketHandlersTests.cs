using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.GenerateTicket;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.ConsumePreviewTicket;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class PdfPreviewTicketHandlersTests
{
    private readonly IMemoryCache _cache;
    private readonly Mock<IMediator> _mediatorMock;

    public PdfPreviewTicketHandlersTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task GenerateTicket_ShouldStoreIdInCache_AndReturnTicketGuid()
    {
        // Arrange
        var handler = new GeneratePdfPreviewTicketHandler(_cache);
        var command = new GeneratePdfPreviewTicketCommand(99);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.Value));

        var inCache = _cache.TryGetValue($"PdfTicket_{result.Value}", out long cachedId);
        Assert.True(inCache);
        Assert.Equal(99, cachedId);
    }

    [Fact]
    public async Task ConsumeTicket_ShouldReturnFileDto_AndBurnTicket()
    {
        // Arrange
        var ticket = "test-ticket-123";
        var expectedPdfId = 42L;
        _cache.Set($"PdfTicket_{ticket}", expectedPdfId);

        var expectedDto = new PdfReportFileDto
        {
            FileName = "Test.pdf",
            FileStream = new MemoryStream()
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetPdfReportPreviewByIdQuery>(q => q.Id == expectedPdfId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedDto));

        var handler = new ConsumePdfPreviewTicketHandler(_cache, _mediatorMock.Object);
        var query = new ConsumePdfPreviewTicketQuery(ticket);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Test.pdf", result.Value.FileName);

        var stillInCache = _cache.TryGetValue($"PdfTicket_{ticket}", out _);
        Assert.False(stillInCache);
    }

    [Fact]
    public async Task ConsumeTicket_ShouldFail_WhenTicketIsInvalid()
    {
        // Arrange
        var handler = new ConsumePdfPreviewTicketHandler(_cache, _mediatorMock.Object);
        var query = new ConsumePdfPreviewTicketQuery("invalid-ticket");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Invalid or expired", result.Errors[0].Message);
    }
}
