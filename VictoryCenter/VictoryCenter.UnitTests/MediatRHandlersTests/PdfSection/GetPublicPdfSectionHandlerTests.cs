using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Mapping.Localization.Languages;
using VictoryCenter.BLL.Mapping.PdfSection;
using VictoryCenter.BLL.Queries.Public.PdfSection.Get;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfSection;

using PdfSection = DAL.Entities.PdfSection;

public class GetPublicPdfSectionHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly IMapper _mapper;

    public GetPublicPdfSectionHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PublicPdfSectionProfile>();
            cfg.AddProfile<LocalizationsLanguageProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_SectionExistsWithLocalizations_ReturnsMappedPublicDto()
    {
        // Arrange
        var section = new PdfSection
        {
            Id = 1,
            Title = "Оновлений заголовок",
            Description = "Оновлений короткий опис",
            CreatedAt = DateTimeOffset.UtcNow,
            Localizations =
            [
                new PdfSectionLocalization
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Title = "Updated Title",
                    Description = "Updated Description",
                    TranslationStatus = TranslationStatus.Outdated,
                    Language = new LocalizationLanguage { Id = 2, Code = "en", Name = "English" },
                },
            ],
        };

        _mockRepo
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
            .ReturnsAsync(section);

        var handler = new GetPublicPdfSectionHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetPublicPdfSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Оновлений заголовок", result.Value.Title);
        Assert.Equal("Оновлений короткий опис", result.Value.Description);
        var localization = Assert.Single(result.Value.Localizations);
        Assert.Equal(2, localization.Language.Id);
        Assert.Equal("en", localization.Language.Code);
        Assert.Equal("Updated Title", localization.Title);
        Assert.Equal("Updated Description", localization.Description);
    }

    [Fact]
    public async Task Handle_SectionExistsWithNullLanguageLocalization_SkipsLocalizationWithoutLanguage()
    {
        // Arrange
        var section = new PdfSection
        {
            Id = 1,
            Title = "Заголовок",
            Description = "Опис",
            CreatedAt = DateTimeOffset.UtcNow,
            Localizations =
            [
                new PdfSectionLocalization
                {
                    EntityId = 1,
                    LanguageId = 2,
                    Title = "Missing language",
                    Description = "Missing language desc",
                    Language = null!,
                },
            ],
        };

        _mockRepo
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
            .ReturnsAsync(section);

        var handler = new GetPublicPdfSectionHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetPublicPdfSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Localizations);
    }

    [Fact]
    public async Task Handle_NoSection_ReturnsNotFoundFailResult()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.PdfSectionRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PdfSection>>()))
            .ReturnsAsync((PdfSection?)null);

        var handler = new GetPublicPdfSectionHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetPublicPdfSectionQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.HasError<NotFoundError>());
        Assert.Contains(PdfSectionConstants.SectionNotFound, result.Errors.Select(e => e.Message));
    }
}
