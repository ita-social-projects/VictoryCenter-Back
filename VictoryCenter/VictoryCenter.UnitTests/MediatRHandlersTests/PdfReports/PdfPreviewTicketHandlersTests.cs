using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.GenerateTicket;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Interfaces.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.ConsumePreviewTicket;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class PdfPreviewTicketHandlersTests
{
    private readonly IMemoryCache _cache;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IPdfTicketStore> _ticketStoreMock;

    public PdfPreviewTicketHandlersTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _mediatorMock = new Mock<IMediator>();
        _ticketStoreMock = new Mock<IPdfTicketStore>();
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

        var expectedDto = new PdfReportFileDto
        {
            FileName = "Test.pdf",
            FileStream = new MemoryStream()
        };

        long outId = expectedPdfId;
        _ticketStoreMock
            .Setup(t => t.TryConsumeTicket(ticket, out outId))
            .Returns(true);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetPdfReportPreviewByIdQuery>(q => q.Id == expectedPdfId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedDto));

        var handler = new ConsumePdfPreviewTicketHandler(_ticketStoreMock.Object, _mediatorMock.Object);
        var query = new ConsumePdfPreviewTicketQuery(ticket);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Test.pdf", result.Value.FileName);

        _ticketStoreMock.Verify(t => t.TryConsumeTicket(ticket, out outId), Times.Once);
    }

    [Fact]
    public async Task ConsumeTicket_ShouldFail_WhenTicketIsInvalid()
    {
        // Arrange
        var ticket = "invalid-ticket";
        long outId;

        _ticketStoreMock
            .Setup(t => t.TryConsumeTicket(ticket, out outId))
            .Returns(false);

        var handler = new ConsumePdfPreviewTicketHandler(_ticketStoreMock.Object, _mediatorMock.Object);
        var query = new ConsumePdfPreviewTicketQuery(ticket);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Invalid or expired", result.Errors[0].Message);
    }
}
