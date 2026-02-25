using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class GetAllPdfReportsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;

    private readonly List<PdfReport> _testPdfReports;
    private readonly List<PdfReportDto> _testPdfReportDtos;

    public GetAllPdfReportsHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();

        _testPdfReports =
        [
            new PdfReport { Id = 1, Name = "Звіт 2024", BlobName = "abc.pdf", Priority = 1, FileSizeBytes = 1024, CreatedAt = DateTimeOffset.UtcNow },
            new PdfReport { Id = 2, Name = "Звіт 2025", BlobName = "def.pdf", Priority = 2, FileSizeBytes = 2048, CreatedAt = DateTimeOffset.UtcNow },
        ];

        _testPdfReportDtos =
        [
            new PdfReportDto { Id = 1, Name = "Звіт 2024", BlobName = "abc.pdf", Priority = 1, FileSizeBytes = 1024, CreatedAt = DateTimeOffset.UtcNow },
            new PdfReportDto { Id = 2, Name = "Звіт 2025", BlobName = "def.pdf", Priority = 2, FileSizeBytes = 2048, CreatedAt = DateTimeOffset.UtcNow },
        ];
    }

    [Fact]
    public async Task Handle_ShouldReturnAllPdfReportsOrderedByPriority()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReports);

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.CountAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(2);

        _mockMapper.Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns(_testPdfReportDtos);

        var query = new GetAllPdfReportsQuery(new BaseFilterDto { Offset = 0, Limit = 20 });

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Length);
        Assert.Equal(2, result.Value.TotalItemsCount);

        _mockRepositoryWrapper.Verify(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.PdfReportRepository.CountAsync(It.IsAny<QueryOptions<PdfReport>>()), Times.Once);
        _mockMapper.Verify(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyList_ShouldReturnEmptyPaginationResult()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync([]);

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.CountAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(0);

        _mockMapper.Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns([]);

        var query = new GetAllPdfReportsQuery(new BaseFilterDto { Offset = 0, Limit = 20 });

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalItemsCount);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldPassCorrectOptionsToRepository()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync([_testPdfReports[0]]);

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.CountAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(2);

        _mockMapper.Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns([_testPdfReportDtos[0]]);

        var query = new GetAllPdfReportsQuery(new BaseFilterDto { Offset = 1, Limit = 1 });

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
        Assert.Equal(2, result.Value.TotalItemsCount);

        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetAllAsync(
            It.Is<QueryOptions<PdfReport>>(o => o.Offset == 1 && o.Limit == 1)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NullOffsetAndLimit_ShouldUseDefaultValues()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.GetAllAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(_testPdfReports);

        _mockRepositoryWrapper.Setup(x => x.PdfReportRepository.CountAsync(It.IsAny<QueryOptions<PdfReport>>()))
            .ReturnsAsync(2);

        _mockMapper.Setup(x => x.Map<List<PdfReportDto>>(It.IsAny<IEnumerable<PdfReport>>()))
            .Returns(_testPdfReportDtos);

        var query = new GetAllPdfReportsQuery(new BaseFilterDto { Offset = null, Limit = null });

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepositoryWrapper.Verify(
            x => x.PdfReportRepository.GetAllAsync(
            It.Is<QueryOptions<PdfReport>>(o => o.Offset == 0 && o.Limit == 20)),
            Times.Once);
    }

    private GetAllPdfReportsHandler CreateHandler() =>
        new(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object);
}
