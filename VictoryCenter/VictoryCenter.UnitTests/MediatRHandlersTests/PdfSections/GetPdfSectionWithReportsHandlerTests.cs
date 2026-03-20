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
    }

    [Fact]
    public async Task Handle_SectionExists_ReturnsDtoWithTitleAndDescription()
    {
        // Arrange
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_pdfSection);

        var handler = new GetPdfSectionWithReportsHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionWithReportsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_pdfSection.Title, result.Value.Title);
        Assert.Equal(_pdfSection.Description, result.Value.Description);
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
    public async Task Handle_SectionExists_ReturnsDtoWithCorrectData()
    {
        // Arrange
        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(_pdfSection);

        var handler = new GetPdfSectionWithReportsHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionWithReportsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_pdfSection.Title, result.Value.Title);
        Assert.Equal(_pdfSection.Description, result.Value.Description);
    }
}
