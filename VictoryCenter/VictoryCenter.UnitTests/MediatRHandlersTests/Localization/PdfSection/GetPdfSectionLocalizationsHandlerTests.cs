using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Queries.Admin.Localization.PdfSection.Get;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PdfSection;

public class GetPdfSectionLocalizationsHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepository;

    private readonly PdfSectionEntity _testSection = new()
    {
        Id = 1,
        Title = "Test Title",
        Description = "Test Description"
    };

    private readonly List<PdfSectionLocalization> _testLocalizations =
    [
        new()
        {
            EntityId = 1,
            LanguageId = 1,
            Title = "Test Title EN",
            Description = "Test Description EN",
            TranslationStatus = TranslationStatus.Relevant
        }

    ];

    private readonly List<PdfSectionLocalizationDto> _testLocalizationDtos =
    [
        new()
        {
            LanguageId = 1,
            LocalizationInfoDto = new() { Id = 1, Code = "en" },
            Title = "Test Title EN",
            Description = "Test Description EN",
            TranslationStatus = TranslationStatus.Relevant
        }

    ];

    public GetPdfSectionLocalizationsHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepository = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = CreateHandler();
        var query = new GetPdfSectionLocalizationsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(_testLocalizationDtos[0].Title, result.Value[0].Title);
        Assert.Equal(_testLocalizationDtos[0].Description, result.Value[0].Description);
        _mockRepository.Verify(
            r => r.PdfSectionLocalizationsRepository
            .GetAllAsync(It.IsAny<QueryOptions<PdfSectionLocalization>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoLocalizations()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockRepository
            .Setup(r => r.PdfSectionLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<PdfSectionLocalization>>()))
            .ReturnsAsync([]);
        _mockMapper
            .Setup(m => m.Map<List<PdfSectionLocalizationDto>>(It.IsAny<IEnumerable<PdfSectionLocalization>>()))
            .Returns([]);

        var handler = CreateHandler();
        var query = new GetPdfSectionLocalizationsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPdfSectionNotFound()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync((PdfSectionEntity?)null);

        var handler = CreateHandler();
        var query = new GetPdfSectionLocalizationsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors[0].Message);
        _mockRepository.Verify(
            r => r.PdfSectionLocalizationsRepository
            .GetAllAsync(It.IsAny<QueryOptions<PdfSectionLocalization>>()), Times.Never);
    }

    private GetPdfSectionLocalizationsHandler CreateHandler() =>
        new(_mockMapper.Object, _mockRepository.Object);

    private void SetupDependencies()
    {
        _mockRepository
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSectionEntity>>()))
            .ReturnsAsync(_testSection);
        _mockRepository
            .Setup(r => r.PdfSectionLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<PdfSectionLocalization>>()))
            .ReturnsAsync(_testLocalizations);
        _mockMapper
            .Setup(m => m.Map<List<PdfSectionLocalizationDto>>(It.IsAny<IEnumerable<PdfSectionLocalization>>()))
            .Returns(_testLocalizationDtos);
    }
}
