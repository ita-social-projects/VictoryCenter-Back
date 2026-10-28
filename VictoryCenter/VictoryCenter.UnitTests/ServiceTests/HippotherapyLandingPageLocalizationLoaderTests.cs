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

        var introEntities = new List<HippotherapyLandingPageIntroSectionLocalization>
        {
            new() { EntityId = page.IntroSection.Id }
        };
        var introRepoMock = SetupRepo(r => r.HippotherapyLandingPageIntroSectionLocalizationsRepository);
        introRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageIntroSectionLocalization>>()))
            .ReturnsAsync(introEntities);

        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageIntroSectionLocalizationDto>>(introEntities))
            .Returns([new HippotherapyLandingPageIntroSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Intro" }]);

        var advantagesEntities = new List<HippotherapyLandingPageAdvantagesSectionLocalization>
        {
            new() { EntityId = page.AdvantagesSection.Id }
        };
        var advantagesSectionDtos = new List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>
        {
            new() { LocalizationInfoDto = lang, Title = "Adv" }
        };

        var advantagesRepoMock = SetupRepo(r => r.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository);
        advantagesRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionLocalization>>()))
            .ReturnsAsync(advantagesEntities);

        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(advantagesEntities))
            .Returns(advantagesSectionDtos);

        var cards = new List<HippotherapyLandingPageAdvantageCard>
        {
            new() { Id = 100, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 1 },
            new() { Id = 200, AdvantagesSectionId = page.AdvantagesSection.Id, Priority = 2 },
        };

        _repoWrapperMock
            .Setup(r => r.HippotherapyLandingPageAdvantageCardsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCard>>()))
            .ReturnsAsync(cards);

        var cardLoc = new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 100, LanguageId = 1 };
        _repoWrapperMock
            .Setup(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>>()))
            .ReturnsAsync([cardLoc]);

        _mapperMock
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(cardLoc))
            .Returns(new HippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 100, Description = "Card 100" });

        var loader = CreateLoader();

        // Act
        var result = await loader.LoadAsync(page, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Intro);
        Assert.Single(result.Advantages);

        var advantageSection = result.Advantages[0];
        Assert.Equal(2, advantageSection.Cards.Count);
        Assert.Equal(100, advantageSection.Cards[0].CardId);
        Assert.Equal("Card 100", advantageSection.Cards[0].Description);

        Assert.Equal(200, advantageSection.Cards[1].CardId);
        Assert.Equal(string.Empty, advantageSection.Cards[1].Description);
    }

    [Fact]
    public async Task LoadAsync_WhenComplexSectionHasNoLocalizations_ShouldReturnEmptyListWithoutQueryingCards()
    {
        // Arrange
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var loader = CreateLoader();

        // Act
        var result = await loader.LoadAsync(page, CancellationToken.None);

        // Assert
        Assert.Empty(result.Advantages);
        _repoWrapperMock.Verify(
            r => r.HippotherapyLandingPageAdvantageCardsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCard>>()),
            Times.Never);
    }

    private HippotherapyLandingPageLocalizationLoader CreateLoader() =>
        new(_repoWrapperMock.Object, _mapperMock.Object);

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

        SetupRepo(r => r.HippotherapyLandingPageAdvantageCardsRepository);
        SetupRepo(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository);
        SetupRepo(r => r.HippotherapyLandingPageParticipantCardsRepository);
        SetupRepo(r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository);
        SetupRepo(r => r.HippotherapyLandingPageScientificReferencesRepository);
        SetupRepo(r => r.HippotherapyLandingPageScientificReferenceLocalizationsRepository);
        SetupRepo(r => r.HippotherapyLandingPageEthicsPrinciplesRepository);
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
