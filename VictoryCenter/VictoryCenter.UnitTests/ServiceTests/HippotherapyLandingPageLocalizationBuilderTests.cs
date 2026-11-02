using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.ServiceTests;

public class HippotherapyLandingPageLocalizationBuilderTests
{
    private const long LanguageId = 2;

    private readonly Mock<IMapper> _mapperMock = new();
    private readonly LocalizationInfoDto _language = HippotherapyLandingPageTestDataHelper.CreateLanguage(LanguageId);

    public HippotherapyLandingPageLocalizationBuilderTests()
    {
        MapSections();
    }

    [Fact]
    public void Build_WhenPageIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CreateBuilder().Build(null!));
    }

    [Fact]
    public void Build_WhenSectionMissing_ShouldThrowArgumentException()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.EthicsSection = null;

        Assert.Throws<ArgumentException>(() => CreateBuilder().Build(page));
    }

    [Fact]
    public void Build_WhenNoSectionLocalizations_ShouldReturnEmptySourceWithoutMappingItems()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();

        var source = CreateBuilder().Build(page);

        Assert.Empty(source.Intro);
        Assert.Empty(source.Advantages);
        Assert.Empty(source.Participants);
        Assert.Empty(source.ScientificReferences);
        Assert.Empty(source.Ethics);
        _mapperMock.Verify(
            m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void Build_ShouldFillAdvantageCardsInPriorityOrderWithEmptyItemForMissingTranslation()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.AdvantagesSection!.AdvantageCards =
        [
            new HippotherapyLandingPageAdvantageCard { Id = 100, Priority = 2 },
            new HippotherapyLandingPageAdvantageCard { Id = 200, Priority = 1 },
        ];
        var translated = new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 100, LanguageId = LanguageId };
        page.AdvantagesSection.AdvantageCards.First(c => c.Id == 100).Localizations = [translated];
        page.AdvantagesSection.Localizations = [new HippotherapyLandingPageAdvantagesSectionLocalization()];

        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns([new HippotherapyLandingPageAdvantagesSectionLocalizationDto { LocalizationInfoDto = _language }]);
        _mapperMock
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(translated))
            .Returns(new HippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 100, Description = "Card 100" });

        var section = Assert.Single(CreateBuilder().Build(page).Advantages);

        Assert.Equal([200L, 100L], section.Cards.Select(c => c.CardId).ToArray());
        Assert.Equal(string.Empty, section.Cards[0].Description);
        Assert.Equal("Card 100", section.Cards[1].Description);
    }

    [Fact]
    public void Build_WhenItemTranslationsAreDuplicated_ShouldUseFirstAndNotThrow()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var first = new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 100, LanguageId = LanguageId };
        var duplicate = new HippotherapyLandingPageAdvantageCardLocalization { EntityId = 100, LanguageId = LanguageId };
        page.AdvantagesSection!.AdvantageCards =
            [new HippotherapyLandingPageAdvantageCard { Id = 100, Priority = 1, Localizations = [first, duplicate] }];
        page.AdvantagesSection.Localizations = [new HippotherapyLandingPageAdvantagesSectionLocalization()];

        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns([new HippotherapyLandingPageAdvantagesSectionLocalizationDto { LocalizationInfoDto = _language }]);
        _mapperMock
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(first))
            .Returns(new HippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 100, Description = "first" });

        var card = Assert.Single(Assert.Single(CreateBuilder().Build(page).Advantages).Cards);

        Assert.Equal("first", card.Description);
    }

    [Fact]
    public void Build_WhenSectionHasTranslationButNoItems_ShouldReturnSectionWithEmptyItems()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.ParticipantsSection!.ParticipantCards = [];
        page.ParticipantsSection.Localizations = [new HippotherapyLandingPageParticipantsSectionLocalization()];
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns([new HippotherapyLandingPageParticipantsSectionLocalizationDto { LocalizationInfoDto = _language }]);

        var section = Assert.Single(CreateBuilder().Build(page).Participants);

        Assert.Empty(section.Cards);
    }

    [Fact]
    public void Build_ShouldKeepItemsOfDifferentLanguagesSeparate()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var otherLanguage = HippotherapyLandingPageTestDataHelper.CreateLanguage(3);
        var translation = new HippotherapyLandingPageParticipantCardLocalization { EntityId = 10, LanguageId = 3 };
        page.ParticipantsSection!.ParticipantCards =
            [new HippotherapyLandingPageParticipantCard { Id = 10, Priority = 1, Localizations = [translation] }];
        page.ParticipantsSection.Localizations = [new HippotherapyLandingPageParticipantsSectionLocalization()];

        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns(
            [
                new HippotherapyLandingPageParticipantsSectionLocalizationDto { LocalizationInfoDto = _language },
                new HippotherapyLandingPageParticipantsSectionLocalizationDto { LocalizationInfoDto = otherLanguage },
            ]);
        _mapperMock
            .Setup(m => m.Map<HippotherapyLandingPageParticipantCardLocalizationItemDto>(translation))
            .Returns(new HippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 10, Description = "L3" });

        var sections = CreateBuilder().Build(page).Participants;

        Assert.Equal(string.Empty, sections.Single(s => s.LocalizationInfoDto.Id == LanguageId).Cards[0].Description);
        Assert.Equal("L3", sections.Single(s => s.LocalizationInfoDto.Id == 3).Cards[0].Description);
    }

    [Fact]
    public void Build_ShouldFillReferencesAndPrinciplesByIdWithEmptyFallback()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.ScientificReferencesSection!.ScientificReferences =
            [new HippotherapyLandingPageScientificReference { Id = 5, Priority = 1 }];
        page.ScientificReferencesSection.Localizations = [new HippotherapyLandingPageScientificReferencesSectionLocalization()];
        page.EthicsSection!.EthicsPrinciples =
            [new HippotherapyLandingPageEthicsPrinciple { Id = 7, Priority = 1 }];
        page.EthicsSection.Localizations = [new HippotherapyLandingPageEthicsSectionLocalization()];

        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns([new HippotherapyLandingPageScientificReferencesSectionLocalizationDto { LocalizationInfoDto = _language }]);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns([new HippotherapyLandingPageEthicsSectionLocalizationDto { LocalizationInfoDto = _language }]);

        var source = CreateBuilder().Build(page);

        var reference = Assert.Single(Assert.Single(source.ScientificReferences).References);
        Assert.Equal(5, reference.ReferenceId);
        Assert.Equal(string.Empty, reference.Name);
        var principle = Assert.Single(Assert.Single(source.Ethics).Principles);
        Assert.Equal(7, principle.PrincipleId);
        Assert.Equal(string.Empty, principle.Text);
    }

    [Fact]
    public void Build_ShouldMapSimpleSectionLocalizations()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.IntroSection!.Localizations = [new HippotherapyLandingPageIntroSectionLocalization()];
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageIntroSectionLocalizationDto>>(It.IsAny<object>()))
            .Returns([new HippotherapyLandingPageIntroSectionLocalizationDto { LocalizationInfoDto = _language, Title = "T" }]);

        var intro = Assert.Single(CreateBuilder().Build(page).Intro);

        Assert.Equal("T", intro.Title);
    }

    private HippotherapyLandingPageLocalizationBuilder CreateBuilder() => new(_mapperMock.Object);

    private void MapSections()
    {
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageIntroSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection.HippotherapyLandingPageDescriptionSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection.HippotherapyLandingPageQuoteSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection.HippotherapyLandingPageHippoventionSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection.HippotherapyLandingPageAnalysisSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
        _mapperMock
            .Setup(m => m.Map<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>(It.IsAny<object>())).Returns([]);
    }
}
