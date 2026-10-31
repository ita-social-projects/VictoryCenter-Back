using System.Linq.Expressions;
using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.ServiceTests;

public class HippotherapyLandingPageLocalizationLoaderTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public HippotherapyLandingPageLocalizationLoaderTests()
    {
        SetupDefaults();
    }

    [Fact]
    public async Task LoadAsync_WhenSectionsAndItemsExist_ShouldMapAndReturnPopulatedSource()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var lang = HippotherapyLandingPageTestDataHelper.CreateLanguage(1);
        SetupIntro(page, lang);
        SetupAdvantages(page, lang, cardLocalizations: [CreateAdvantageCardLocalization(100, 1)]);
        SetAdvantageCards(
            page,
            new HippotherapyLandingPageAdvantageCard { Id = 100, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 1 },
            new HippotherapyLandingPageAdvantageCard { Id = 200, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 2 });

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        Assert.Single(result.Intro);
        var advantages = Assert.Single(result.Advantages);
        Assert.Equal(2, advantages.Cards.Count);
        Assert.Equal(100, advantages.Cards[0].CardId);
        Assert.Equal("Card 100", advantages.Cards[0].Description);
        Assert.Equal(200, advantages.Cards[1].CardId);
        Assert.Equal(string.Empty, advantages.Cards[1].Description);
    }

    [Fact]
    public async Task LoadAsync_ShouldOrderItemsByPriorityFromPageGraph()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var lang = HippotherapyLandingPageTestDataHelper.CreateLanguage(1);
        SetupAdvantages(page, lang, cardLocalizations: []);
        SetAdvantageCards(
            page,
            new HippotherapyLandingPageAdvantageCard { Id = 100, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 2 },
            new HippotherapyLandingPageAdvantageCard { Id = 200, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 1 });

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        var advantages = Assert.Single(result.Advantages);
        Assert.Equal([200L, 100L], advantages.Cards.Select(c => c.CardId).ToArray());
    }

    [Fact]
    public async Task LoadAsync_WhenItemLocalizationIsMissing_ShouldReturnItemWithEmptyText()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var lang = HippotherapyLandingPageTestDataHelper.CreateLanguage(1);
        SetupAdvantages(page, lang, cardLocalizations: []);
        SetAdvantageCards(
            page,
            new HippotherapyLandingPageAdvantageCard { Id = 100, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 1 });

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        var card = Assert.Single(Assert.Single(result.Advantages).Cards);
        Assert.Equal(100, card.CardId);
        Assert.Equal(string.Empty, card.Description);
    }

    [Fact]
    public async Task LoadAsync_WhenItemLocalizationsContainDuplicates_ShouldNotThrowAndUseFirst()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var lang = HippotherapyLandingPageTestDataHelper.CreateLanguage(1);
        var first = CreateAdvantageCardLocalization(100, 1);
        var duplicate = CreateAdvantageCardLocalization(100, 1);
        SetupAdvantages(page, lang, cardLocalizations: [first, duplicate]);
        SetAdvantageCards(
            page,
            new HippotherapyLandingPageAdvantageCard { Id = 100, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 1 });

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        var card = Assert.Single(Assert.Single(result.Advantages).Cards);
        Assert.Equal("Card 100", card.Description);
    }

    [Fact]
    public async Task LoadAsync_WhenParticipantsHaveLocalizations_ShouldFillCardsInGraphOrder()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var lang = HippotherapyLandingPageTestDataHelper.CreateLanguage(1);

        var sectionEntities = new List<HippotherapyLandingPageParticipantsSectionLocalization>
        {
            new() { EntityId = page.ParticipantsSection.Id },
        };
        SetupRepo(r => r.HippotherapyLandingPageParticipantsSectionLocalizationsRepository)
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageParticipantsSectionLocalization>>()))
            .ReturnsAsync(sectionEntities);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>>(sectionEntities))
            .Returns([new HippotherapyLandingPageParticipantsSectionLocalizationDto { LocalizationInfoDto = lang, Title = "P" }]);

        var cardLocalization = new HippotherapyLandingPageParticipantCardLocalization { EntityId = 20, LanguageId = 1 };
        SetupRepo(r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository)
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCardLocalization>>()))
            .ReturnsAsync([cardLocalization]);
        _mapperMock
            .Setup(m => m.Map<HippotherapyLandingPageParticipantCardLocalizationItemDto>(cardLocalization))
            .Returns(new HippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 20, Description = "Card 20" });

        page.ParticipantsSection.ParticipantCards =
        [
            new HippotherapyLandingPageParticipantCard { Id = 10, ParticipantsSectionId = page.ParticipantsSection.Id, Priority = 2 },
            new HippotherapyLandingPageParticipantCard { Id = 20, ParticipantsSectionId = page.ParticipantsSection.Id, Priority = 1 },
        ];

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        var participants = Assert.Single(result.Participants);
        Assert.Equal([20L, 10L], participants.Cards.Select(c => c.CardId).ToArray());
        Assert.Equal("Card 20", participants.Cards[0].Description);
        Assert.Equal(string.Empty, participants.Cards[1].Description);
    }

    [Fact]
    public async Task LoadAsync_WhenComplexSectionHasNoLocalizations_ShouldReturnEmptyListWithoutQueryingItems()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        Assert.Empty(result.Advantages);
        VerifyItemLocalizationsNeverQueried();
    }

    [Fact]
    public async Task LoadAsync_WhenSectionHasLocalizationsButNoItems_ShouldNotQueryItemLocalizations()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var lang = HippotherapyLandingPageTestDataHelper.CreateLanguage(1);
        SetupAdvantages(page, lang, cardLocalizations: []);
        SetAdvantageCards(page);

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        Assert.Single(result.Advantages);
        VerifyItemLocalizationsNeverQueried();
    }

    [Fact]
    public async Task LoadAsync_WhenAllSectionsAreEmpty_ShouldReturnEmptySource()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();

        var result = await CreateLoader().LoadAsync(page, CancellationToken.None);

        Assert.Empty(result.Intro);
        Assert.Empty(result.Description);
        Assert.Empty(result.Quote);
        Assert.Empty(result.Hippovention);
        Assert.Empty(result.HippoventionCenter);
        Assert.Empty(result.Advantages);
        Assert.Empty(result.Analysis);
        Assert.Empty(result.ScientificReferences);
        Assert.Empty(result.AnotherQuote);
        Assert.Empty(result.Participants);
        Assert.Empty(result.Ethics);
    }

    [Fact]
    public async Task LoadAsync_WhenPageIsNull_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => CreateLoader().LoadAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task LoadAsync_WhenSectionIsMissing_ShouldThrowArgumentException()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.IntroSection = null!;

        await Assert.ThrowsAsync<ArgumentException>(
            () => CreateLoader().LoadAsync(page, CancellationToken.None));
    }

    private HippotherapyLandingPageLocalizationLoader CreateLoader() =>
        new(_repoWrapperMock.Object, _mapperMock.Object);

    private static HippotherapyLandingPageAdvantageCardLocalization CreateAdvantageCardLocalization(
        long cardId,
        long languageId) =>
        new() { EntityId = cardId, LanguageId = languageId };

    private static void SetAdvantageCards(
        DAL.Entities.HippotherapyLandingPage page,
        params HippotherapyLandingPageAdvantageCard[] cards)
    {
        page.AdvantagesSection.AdvantageCards = [.. cards];
    }

    private void SetupIntro(DAL.Entities.HippotherapyLandingPage page, VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto lang)
    {
        var entities = new List<HippotherapyLandingPageIntroSectionLocalization>
        {
            new() { EntityId = page.IntroSection.Id },
        };
        SetupRepo(r => r.HippotherapyLandingPageIntroSectionLocalizationsRepository)
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageIntroSectionLocalization>>()))
            .ReturnsAsync(entities);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageIntroSectionLocalizationDto>>(entities))
            .Returns([new HippotherapyLandingPageIntroSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Intro" }]);
    }

    private void SetupAdvantages(
        DAL.Entities.HippotherapyLandingPage page,
        VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto lang,
        List<HippotherapyLandingPageAdvantageCardLocalization> cardLocalizations)
    {
        var sectionEntities = new List<HippotherapyLandingPageAdvantagesSectionLocalization>
        {
            new() { EntityId = page.AdvantagesSection.Id },
        };
        SetupRepo(r => r.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository)
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionLocalization>>()))
            .ReturnsAsync(sectionEntities);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(sectionEntities))
            .Returns([new HippotherapyLandingPageAdvantagesSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Adv" }]);

        SetupRepo(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository)
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>>()))
            .ReturnsAsync(cardLocalizations);

        foreach (var localization in cardLocalizations)
        {
            _mapperMock
                .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(localization))
                .Returns(new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                {
                    CardId = localization.EntityId,
                    Description = $"Card {localization.EntityId}",
                });
        }
    }

    private void VerifyItemLocalizationsNeverQueried()
    {
        _repoWrapperMock.Verify(
            r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>>()),
            Times.Never);
        _repoWrapperMock.Verify(
            r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCardLocalization>>()),
            Times.Never);
    }

    private void SetupDefaults()
    {
        SetupRepo(r => r.HippotherapyLandingPageIntroSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageIntroSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageDescriptionSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageDescriptionSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageQuoteSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageQuoteSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageHippoventionSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageHippoventionSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageHippoventionCenterSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageAnalysisSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageAnalysisSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageAnotherQuoteSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageAdvantagesSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageParticipantsSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageParticipantsSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>();
        SetupRepo(r => r.HippotherapyLandingPageEthicsSectionLocalizationsRepository);
        SetupEmptyMapping<HippotherapyLandingPageEthicsSectionLocalizationDto>();

        SetupRepo(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository);
        SetupRepo(r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository);
        SetupRepo(r => r.HippotherapyLandingPageScientificReferenceLocalizationsRepository);
        SetupRepo(r => r.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository);
    }

    private Mock<TRepo> SetupRepo<TRepo>(Expression<Func<IRepositoryWrapper, TRepo>> repoProperty)
        where TRepo : class
    {
        var repoMock = new Mock<TRepo>();
        _repoWrapperMock.Setup(repoProperty).Returns(repoMock.Object);
        return repoMock;
    }

    private void SetupEmptyMapping<TDto>()
    {
        _mapperMock
            .Setup(m => m.Map<List<TDto>>(It.IsAny<object>()))
            .Returns([]);
    }
}
