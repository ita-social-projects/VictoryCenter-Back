using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Admin.PdfSection.Get;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfSection;

using PdfSection = DAL.Entities.PdfSection;

public class GetPdfSectionHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly PdfSection _pdfSection;

    public GetPdfSectionHandlerTests()
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

        var handler = new GetPdfSectionHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionQuery(), CancellationToken.None);

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

        var handler = new GetPdfSectionHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionQuery(), CancellationToken.None);

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

        var handler = new GetPdfSectionHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_pdfSection.Title, result.Value.Title);
        Assert.Equal(_pdfSection.Description, result.Value.Description);
    }

    [Fact]
    public async Task Handle_SectionWithLocalizations_ReturnsDtoWithLocalizations()
    {
        // Arrange
        var sectionWithLocalizations = new PdfSection
        {
            Id = 1,
            Title = "Тестова секція",
            Description = "Опис тестової секції",
            CreatedAt = DateTimeOffset.UtcNow,
            Localizations =
            [
                new()
            {
                LanguageId = 1,
                Title = "Test section",
                Description = "Test description",
                TranslationStatus = DAL.Enums.TranslationStatus.Relevant,
                Language = new() { Id = 1, Code = "en", Name = "English" }
            },
            ]
        };

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(sectionWithLocalizations);

        var handler = new GetPdfSectionHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Localizations);
        Assert.Single(result.Value.Localizations);
        var loc = result.Value.Localizations.First();
        Assert.Equal(1, loc.LanguageId);
        Assert.Equal("Test section", loc.Title);
        Assert.Equal("Test description", loc.Description);
    }

    [Fact]
    public async Task Handle_SectionWithNoLocalizations_ReturnsDtoWithEmptyLocalizations()
    {
        // Arrange
        var sectionWithNoLocalizations = new PdfSection
        {
            Id = 1,
            Title = "Тестова секція",
            Description = "Опис тестової секції",
            CreatedAt = DateTimeOffset.UtcNow,
            Localizations = []
        };

        _mockRepo.Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
                 .ReturnsAsync(sectionWithNoLocalizations);

        var handler = new GetPdfSectionHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetPdfSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Localizations);
        Assert.Empty(result.Value.Localizations);
    }
}
