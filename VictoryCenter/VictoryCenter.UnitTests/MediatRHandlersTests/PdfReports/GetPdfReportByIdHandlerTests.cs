using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class GetPdfReportByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IPdfService> _mockPdfService;

    private readonly PdfReport _testPdfReport;

    public GetPdfReportByIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockPdfService = new Mock<IPdfService>();

        _testPdfReport = new PdfReport
        {
            Id = 1,
            Name = "Звіт 2024",
            BlobName = "report-2024.pdf",
            Priority = 1,
            FileSizeBytes = 1024,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    [Fact]
    public async Task Handle_ValidPdfReportId_ReturnsPdfStream()
    {
        // Arrange
        var pdfBytes = new byte[] { 37, 80, 68, 70 }; // PDF magic bytes: %PDF
        var pdfStream = new MemoryStream(pdfBytes);

        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReport);

        _mockPdfService
            .Setup(x => x.GetPdfAsync(_testPdfReport.BlobName))
            .ReturnsAsync(pdfStream);

        var query = new GetPdfReportByIdQuery(1);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Same(pdfStream, result.Value);

        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Once);
        _mockPdfService.Verify(
            x => x.GetPdfAsync(_testPdfReport.BlobName),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PdfReportNotFound_ReturnsFailResult()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync((PdfReport?)null);

        var query = new GetPdfReportByIdQuery(999);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(999, typeof(PdfReport)),
            result.Errors.Select(e => e.Message));

        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Once);
        _mockPdfService.Verify(
            x => x.GetPdfAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_PdfServiceThrowsException_ReturnsFailResult()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReport);

        _mockPdfService
            .Setup(x => x.GetPdfAsync(_testPdfReport.BlobName))
            .ThrowsAsync(new IOException("File not found"));

        var query = new GetPdfReportByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<IOException>(() => CreateHandler().Handle(query, CancellationToken.None));

        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()),
            Times.Once);
    }

    private GetPdfReportByIdHandler CreateHandler() =>
        new(_mockRepositoryWrapper.Object, _mockPdfService.Object);
}
