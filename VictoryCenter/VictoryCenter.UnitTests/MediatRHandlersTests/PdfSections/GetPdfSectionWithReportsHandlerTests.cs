using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Admin.PdfSectionWithReport;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfSections;

public class GetPdfSectionWithReportsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly List<PdfReport> _pdfReports;
    private readonly PdfSection _pdfSection;

    public GetPdfSectionWithReportsHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();

        _pdfSection = new PdfSection
        {
            Id = 1,
            Title = "Тестова секція",
            Description = "Опис тестової секції",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _pdfReports = new List<PdfReport>
        {
            new() { Id = 1, Name = "Звіт 2024", BlobName = "file1.pdf", Priority = 1, FileSizeBytes = 1024, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Name = "Звіт 2025", BlobName = "file2.pdf", Priority = 2, FileSizeBytes = 2048, CreatedAt = DateTimeOffset.UtcNow }
        };
    }

    [Fact]
    public async Task Handle_SectionExists_ReturnsDtoWithReports()
    {
        // Arrange
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_pdfSection);

        _mockRepo.Setup(r => r.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
                 .ReturnsAsync(_pdfReports);

        var handler = new GetPdfSectionWithReportsHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionWithReportsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_pdfSection.Title, result.Value.Title);
        Assert.Equal(_pdfSection.Description, result.Value.Description);
        Assert.Equal(2, result.Value.PdfFiles.Count);
        Assert.Equal(_pdfReports[0].Id, result.Value.PdfFiles[0].Id);
        Assert.Equal(_pdfReports[1].Id, result.Value.PdfFiles[1].Id);
    }

    [Fact]
    public async Task Handle_NoSection_ReturnsFailResult()
    {
        // Arrange
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync((PdfSection?)null);

        var handler = new GetPdfSectionWithReportsHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionWithReportsQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(PdfSectionConstants.SectionNotFound, result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_EmptyPdfReports_ReturnsDtoWithEmptyPdfFiles()
    {
        // Arrange
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_pdfSection);

        _mockRepo.Setup(r => r.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
                 .ReturnsAsync(new List<PdfReport>());

        var handler = new GetPdfSectionWithReportsHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionWithReportsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_pdfSection.Title, result.Value.Title);
        Assert.Equal(_pdfSection.Description, result.Value.Description);
        Assert.Empty(result.Value.PdfFiles);
    }
}
