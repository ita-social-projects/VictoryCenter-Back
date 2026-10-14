using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageParticipantsSection.GetByEntityId;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageParticipantsSection;

public class GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdTests
{
    private const long SectionId = 1;
    private const long EnglishId = 2;
    private const long GermanId = 3;

    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();
    private readonly GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdHandler _handler;

    public GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdTests()
    {
        _handler = new GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageParticipantCardLocalizationItemDto>(It.IsAny<HippotherapyLandingPageParticipantCardLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageParticipantCardLocalization)src;
                return new HippotherapyLandingPageParticipantCardLocalizationItemDto
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
        var sectionLocalizations = new List<HippotherapyLandingPageParticipantsSectionLocalization>
        {
            new() { EntityId = SectionId, LanguageId = EnglishId, Title = "Who the programs are for" },
            new() { EntityId = SectionId, LanguageId = GermanId, Title = "Warum dieser Ansatz" },
        };
        SetupSectionLocalizations(sectionLocalizations);
        SetupCards(
        [
            new HippotherapyLandingPageParticipantCard { Id = 10, ParticipantsSectionId = SectionId, Priority = 1 },
            new HippotherapyLandingPageParticipantCard { Id = 11, ParticipantsSectionId = SectionId, Priority = 2 },
        ]);
        SetupCardLocalizations(
        [
            new HippotherapyLandingPageParticipantCardLocalization
            {
                EntityId = 10, LanguageId = EnglishId, Description = "English card 1", TranslationStatus = TranslationStatus.Relevant,
            },
            new HippotherapyLandingPageParticipantCardLocalization
            {
                EntityId = 11, LanguageId = EnglishId, Description = "English card 2", TranslationStatus = TranslationStatus.Outdated,
            },
            new HippotherapyLandingPageParticipantCardLocalization
            {
                EntityId = 10, LanguageId = GermanId, Description = "German card 1", TranslationStatus = TranslationStatus.Relevant,
            },
        ]);

        var result = await _handler.Handle(
            new GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdQuery(SectionId),
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
            new GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdQuery(SectionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _mockRepositoryWrapper.Verify(
            r => r.HippotherapyLandingPageParticipantCardsRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCard>>()),
            Times.Never);
    }

    private void SetupSectionLocalizations(List<HippotherapyLandingPageParticipantsSectionLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantsSectionLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantsSectionLocalization>>()))
            .ReturnsAsync(localizations);
        _mockMapper
            .Setup(m => m.Map<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns(localizations
                .Select(l => new HippotherapyLandingPageParticipantsSectionLocalizationDto
                {
                    EntityId = l.EntityId,
                    LocalizationInfoDto = new LocalizationInfoDto { Id = l.LanguageId },
                    Title = l.Title,
                })
                .ToList());
    }

    private void SetupCards(List<HippotherapyLandingPageParticipantCard> cards)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantCardsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCard>>()))
            .ReturnsAsync(cards);
    }

    private void SetupCardLocalizations(List<HippotherapyLandingPageParticipantCardLocalization> localizations)
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCardLocalization>>()))
            .ReturnsAsync(localizations);
    }
}
