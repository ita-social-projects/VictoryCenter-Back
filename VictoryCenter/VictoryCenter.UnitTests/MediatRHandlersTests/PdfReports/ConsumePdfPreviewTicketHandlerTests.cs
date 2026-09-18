using FluentResults;
using MediatR;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Interfaces.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.ConsumePreviewTicket;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class ConsumePdfPreviewTicketHandlerTests
{
    private readonly Mock<IPdfTicketStore> _mockTicketStore;
    private readonly Mock<IMediator> _mockMediator;

    public ConsumePdfPreviewTicketHandlerTests()
    {
        _mockTicketStore = new Mock<IPdfTicketStore>();
        _mockMediator = new Mock<IMediator>();
    }

    [Fact]
    public async Task Handle_WhenTicketIsInvalidOrExpired_ShouldReturnFailAndNotCallMediator()
    {
        // Arrange
        var request = new ConsumePdfPreviewTicketQuery("invalid-ticket");
        long dummyPdfId = 0;

        _mockTicketStore
            .Setup(x => x.TryConsumeTicket(request.Ticket, out dummyPdfId))
            .Returns(false);

        // Act
        var result = await CreateHandler().Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);

        Assert.Equal(PdfReportConstants.InvalidOrExpiredPreviewTicket, result.Errors.First().Message);

        _mockMediator.Verify(
            x => x.Send(It.IsAny<GetPdfReportPreviewByIdQuery>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTicketIsValid_ShouldCallMediatorAndReturnItsResult()
    {
        // Arrange
        var request = new ConsumePdfPreviewTicketQuery("valid-ticket");
        long expectedPdfId = 42;

        _mockTicketStore
            .Setup(x => x.TryConsumeTicket(request.Ticket, out expectedPdfId))
            .Returns(true);

        var expectedFileDto = new PdfReportFileDto { FileName = "report.pdf" };
        var mediatorResult = Result.Ok(expectedFileDto);

        _mockMediator
            .Setup(x => x.Send(It.Is<GetPdfReportPreviewByIdQuery>(q => q.Id == expectedPdfId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResult);

        // Act
        var result = await CreateHandler().Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(expectedFileDto, result.Value);

        _mockMediator.Verify(
            x => x.Send(It.Is<GetPdfReportPreviewByIdQuery>(q => q.Id == expectedPdfId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private ConsumePdfPreviewTicketHandler CreateHandler() =>
        new(
            _mockTicketStore.Object,
            _mockMediator.Object);
}
