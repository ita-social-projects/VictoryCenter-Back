using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAdvantagesSection.GetByEntityId;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAdvantagesSection;

public class GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdTests
{
    private const long SectionId = 1;
    private const long EnglishId = 2;
    private const long GermanId = 3;

    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();
    private readonly GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdHandler _handler;

    public GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdTests()
    {
        _handler = new GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(It.IsAny<HippotherapyLandingPageAdvantageCardLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageAdvantageCardLocalization)src;
                return new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                {
                    CardId = entity.EntityId,
                    Description = entity.Description,
                    TranslationStatus = entity.TranslationStatus,
                };
            });
    }

    [Fact]
    public async Task Handle_ShouldReturnCardsPerLanguage_WithEmptyItemsForMissingTranslations()
    {
        var sectionLocalizations = new List<HippotherapyLandingPageAdvantagesSectionLocalization>
        {
            new() { EntityId = SectionId, LanguageId = EnglishId, Title = "Why this approach" },
            new() { EntityId = SectionId, LanguageId = GermanId, Title = "Warum dieser Ansatz" },
        };
        SetupSectionLocalizations(sectionLocalizations);
        SetupCards(
        [
            new HippotherapyLandingPageAdvantageCard { Id = 10, AdvantagesSectionId = SectionId, Priority = 1 },
            new HippotherapyLandingPageAdvantageCard { Id = 11, AdvantagesSectionId = SectionId, Priority = 2 },
        ]);
        SetupCardLocalizations(
        [
            new HippotherapyLandingPageAdvantageCardLocalization
            {
                EntityId = 10, LanguageId = EnglishId, Description = "English card 1", TranslationStatus = TranslationStatus.Relevant,
            },
            new HippotherapyLandingPageAdvantageCardLocalization
            {
                EntityId = 11, LanguageId = EnglishId, Description = "English card 2", TranslationStatus = TranslationStatus.Outdated,
            },
            new HippotherapyLandingPageAdvantageCardLocalization
            {
                EntityId = 10, LanguageId = GermanId, Description = "German card 1", TranslationStatus = TranslationStatus.Relevant,
            },
        ]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        var english = result.Value[0];
        Assert.Equal(new[] { 10L, 11L }, english.Cards.Select(c => c.CardId));
        Assert.Equal("English card 1", english.Cards[0].Description);
        Assert.Equal(TranslationStatus.Outdated, english.Cards[1].TranslationStatus);

        var german = result.Value[1];
        Assert.Equal("German card 1", german.Cards[0].Description);
        Assert.Equal(11, german.Cards[1].CardId);
        Assert.Equal(string.Empty, german.Cards[1].Description);
        Assert.Null(german.Cards[1].TranslationStatus);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSectionHasNoLocalizations()
    {
        SetupSectionLocalizations([]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageAdvantageCardsRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCard>>()),
            Times.Never);
    }

    private void SetupSectionLocalizations(List<HippotherapyLandingPageAdvantagesSectionLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionLocalization>>()))
            .ReturnsAsync(localizations);
        _mockMapper
            .Setup(m => m.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns(localizations
                .Select(l => new HippotherapyLandingPageAdvantagesSectionLocalizationDto
                {
                    EntityId = l.EntityId,
                    LocalizationInfoDto = new LocalizationInfoDto { Id = l.LanguageId },
                    Title = l.Title,
                })
                .ToList());
    }

    private void SetupCards(List<HippotherapyLandingPageAdvantageCard> cards)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantageCardsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCard>>()))
            .ReturnsAsync(cards);
    }

    private void SetupCardLocalizations(List<HippotherapyLandingPageAdvantageCardLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>>()))
            .ReturnsAsync(localizations);
    }
}
