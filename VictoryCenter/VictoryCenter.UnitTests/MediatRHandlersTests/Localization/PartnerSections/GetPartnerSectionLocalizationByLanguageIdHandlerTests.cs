using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.PartnerSections.GetByLanguageId;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnerSections;

public class GetPartnerSectionLocalizationByLanguageIdHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly GetPartnerSectionLocalizationByLanguageIdHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly PartnerSection _section = new()
    {
        Id = 1,
        Partners =
        [
            new Partner { Id = 10, PartnersSectionId = 1, Description = "Original description 1" },
            new Partner { Id = 11, PartnersSectionId = 1, Description = "Original description 2" }
        ]
    };

    private readonly PartnerSectionLocalization _sectionLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Localized section title",
        Description = "Localized section description"
    };

    private readonly PartnerSectionLocalizationDto _sectionLocalizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Localized section title",
        Description = "Localized section description"
    };

    public GetPartnerSectionLocalizationByLanguageIdHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new GetPartnerSectionLocalizationByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(_section);
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSectionLocalization>>()))
            .ReturnsAsync(_sectionLocalization);
        _mockMapper.Setup(m => m.Map<PartnerSectionLocalizationDto>(_sectionLocalization)).Returns(_sectionLocalizationDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnSectionWithPartners_WhenSomePartnersAreNotTranslatedYet()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()))
            .ReturnsAsync(
            [
                new PartnerLocalization { EntityId = 10, LanguageId = 2, Description = "Translated", TranslationStatus = TranslationStatus.Relevant }
            ]);

        var query = new GetPartnerSectionLocalizationByLanguageIdQuery(_entityId, _languageId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Partners.Count);

        var translated = result.Value.Partners.Single(p => p.PartnerId == 10);
        Assert.Equal("Translated", translated.Description);
        Assert.Equal(TranslationStatus.Relevant, translated.TranslationStatus);

        var untranslated = result.Value.Partners.Single(p => p.PartnerId == 11);
        Assert.Equal(string.Empty, untranslated.Description);
        Assert.Null(untranslated.TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPartners_WhenSectionHasNoPartners()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(new PartnerSection { Id = 1, Partners = [] });

        var query = new GetPartnerSectionLocalizationByLanguageIdQuery(_entityId, _languageId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Partners);
        _mockRepositoryWrapper.Verify(r => r.PartnerLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<PartnerLocalization>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync((PartnerSection?)null);

        var query = new GetPartnerSectionLocalizationByLanguageIdQuery(_entityId, _languageId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionLocalizationNotFound()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSectionLocalization>>()))
            .ReturnsAsync((PartnerSectionLocalization?)null);

        var query = new GetPartnerSectionLocalizationByLanguageIdQuery(_entityId, _languageId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
