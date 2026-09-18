using Moq;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class GetPdfReportPreviewByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IPdfService> _mockPdfService;

    public GetPdfReportPreviewByIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockPdfService = new Mock<IPdfService>();
    }

    [Fact]
    public async Task Handle_WhenPdfReportExistsAndNameHasNoExtension_ShouldAppendPdfAndReturnOk()
    {
        // Arrange
        long testId = 1;
        var testReport = new PdfReport { Id = testId, BlobName = "blob-123", Name = "Звіт 2024" };
        var testStream = new MemoryStream();

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(testReport);

        _mockPdfService.Setup(x => x.GetPdfAsync(testReport.BlobName))
            .ReturnsAsync(testStream);

        var query = new GetPdfReportPreviewByIdQuery(testId);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Звіт 2024.pdf", result.Value.FileName);
        Assert.Same(testStream, result.Value.FileStream);

        _mockRepositoryWrapper.Verify(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()), Times.Once);
        _mockPdfService.Verify(x => x.GetPdfAsync(testReport.BlobName), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPdfReportExistsAndNameEndsWithPdf_ShouldNotAppendExtensionAndReturnOk()
    {
        // Arrange
        long testId = 1;
        var testReport = new PdfReport { Id = testId, BlobName = "blob-123", Name = "Звіт 2024.pdf" };
        var testStream = new MemoryStream();

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(testReport);

        _mockPdfService.Setup(x => x.GetPdfAsync(testReport.BlobName))
            .ReturnsAsync(testStream);

        var query = new GetPdfReportPreviewByIdQuery(testId);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Звіт 2024.pdf", result.Value.FileName);
    }

    [Fact]
    public async Task Handle_WhenPdfReportNotFoundInDatabase_ShouldReturnFail()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync((PdfReport)null);

        var query = new GetPdfReportPreviewByIdQuery(99);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);

        _mockPdfService.Verify(x => x.GetPdfAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBlobNotFoundExceptionThrown_ShouldReturnFail()
    {
        // Arrange
        long testId = 1;
        var testReport = new PdfReport { Id = testId, BlobName = "blob-123", Name = "Звіт" };

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(testReport);

        _mockPdfService.Setup(x => x.GetPdfAsync(testReport.BlobName))
            .ThrowsAsync(new BlobNotFoundException(testReport.Name, "Blob not found"));

        var query = new GetPdfReportPreviewByIdQuery(testId);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WhenBlobFileSystemExceptionThrown_ShouldReturnFail()
    {
        // Arrange
        var testId = 1;
        var testReport = new PdfReport { Id = testId, BlobName = "blob-123", Name = "Звіт" };

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(testReport);

        _mockPdfService.Setup(x => x.GetPdfAsync(testReport.BlobName))
            .ThrowsAsync(new BlobFileSystemException(testReport.Name, "File system error"));

        var query = new GetPdfReportPreviewByIdQuery(testId);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);
    }

    private GetPdfReportPreviewByIdHandler CreateHandler() =>
        new(
            _mockRepositoryWrapper.Object,
            _mockPdfService.Object);
}
