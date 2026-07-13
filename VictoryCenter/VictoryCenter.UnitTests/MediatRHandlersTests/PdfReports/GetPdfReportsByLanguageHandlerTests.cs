using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetByLanguage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class GetPdfReportsByLanguageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;

    private readonly List<PdfReport> _uaReports;
    private readonly List<PdfReport> _enReports;
    private readonly List<PdfReportDto> _uaReportDtos;
    private readonly List<PdfReportDto> _enReportDtos;

    public GetPdfReportsByLanguageHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();

        _uaReports =
        [
            new PdfReport { Id = 1, Name = "Звіт 2024", BlobName = "ua1.pdf", Priority = 1, FileSizeBytes = 1024, LanguageId = 1, CreatedAt = DateTimeOffset.UtcNow },
            new PdfReport { Id = 2, Name = "Звіт 2025", BlobName = "ua2.pdf", Priority = 2, FileSizeBytes = 2048, LanguageId = 1, CreatedAt = DateTimeOffset.UtcNow },
        ];

        _enReports =
        [
            new PdfReport { Id = 3, Name = "Report 2024", BlobName = "en1.pdf", Priority = 1, FileSizeBytes = 1024, LanguageId = 2, CreatedAt = DateTimeOffset.UtcNow },
        ];

        _uaReportDtos =
        [
            new PdfReportDto { Id = 1, Name = "Звіт 2024", BlobName = "ua1.pdf", Priority = 1, FileSizeBytes = 1024, LanguageId = 1, CreatedAt = DateTimeOffset.UtcNow },
            new PdfReportDto { Id = 2, Name = "Звіт 2025", BlobName = "ua2.pdf", Priority = 2, FileSizeBytes = 2048, LanguageId = 1, CreatedAt = DateTimeOffset.UtcNow },
        ];

        _enReportDtos =
        [
            new PdfReportDto { Id = 3, Name = "Report 2024", BlobName = "en1.pdf", Priority = 1, FileSizeBytes = 1024, LanguageId = 2, CreatedAt = DateTimeOffset.UtcNow },
        ];
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyReportsForRequestedLanguage()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_uaReports);

        _mockMapper
            .Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns(_uaReportDtos);

        var query = new GetPdfReportsByLanguageQuery(1);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, dto => Assert.Equal(1, dto.LanguageId));

        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetAllAsync(
                It.Is<QueryOptions<PdfReport>>(o => o.Filter != null)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassLanguageFilterToRepository()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_enReports);

        _mockMapper
            .Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns(_enReportDtos);

        var query = new GetPdfReportsByLanguageQuery(2);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetAllAsync(
                It.Is<QueryOptions<PdfReport>>(o => o.Filter != null && o.OrderByASC != null)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyList_ShouldReturnEmptyResult()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync([]);

        _mockMapper
            .Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns([]);

        var query = new GetPdfReportsByLanguageQuery(1);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnReportsOrderedByPriority()
    {
        // Arrange
        _mockRepositoryWrapper
            .Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_uaReports);

        _mockMapper
            .Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns(_uaReportDtos);

        var query = new GetPdfReportsByLanguageQuery(1);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetAllAsync(
                It.Is<QueryOptions<PdfReport>>(o => o.OrderByASC != null)),
            Times.Once);
    }

    private GetPdfReportsByLanguageHandler CreateHandler() =>
        new(_mockRepositoryWrapper.Object, _mockMapper.Object);
}
