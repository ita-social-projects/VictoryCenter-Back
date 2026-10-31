using VictoryCenter.BLL.Enums;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Enums;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.ServiceTests;

public class HippotherapyLandingPageLocalizationAggregatorTests
{
    [Fact]
    public void BuildLocalizations_WhenSourceIsEmpty_ShouldReturnEmptyList()
    {
        var result = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(
            HippotherapyLandingPageLocalizationSource.Empty);

        Assert.Empty(result);
    }

    [Fact]
    public void BuildLocalizations_WhenAllSectionsAreRelevant_TranslationStatusShouldBeRelevant()
    {
        var source = HippotherapyLandingPageTestDataHelper.CreateFullLocalizationSource(
            sectionStatus: TranslationStatus.Relevant,
            itemStatus: TranslationStatus.Relevant);

        var result = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source);

        var localized = Assert.Single(result);
        Assert.Equal(TranslationStatus.Relevant, localized.TranslationStatus);
        Assert.NotNull(localized.IntroSection);
        Assert.NotNull(localized.DescriptionSection);
        Assert.NotNull(localized.QuoteSection);
        Assert.NotNull(localized.HippoventionSection);
        Assert.NotNull(localized.HippoventionCenterSection);
        Assert.NotNull(localized.AdvantagesSection);
        Assert.NotNull(localized.AnalysisSection);
        Assert.NotNull(localized.ScientificReferencesSection);
        Assert.NotNull(localized.AnotherQuoteSection);
        Assert.NotNull(localized.ParticipantsSection);
        Assert.NotNull(localized.EthicsSection);
        Assert.Single(localized.AdvantagesSection.Cards);
        Assert.Single(localized.ScientificReferencesSection.References);
        Assert.Single(localized.ParticipantsSection.Cards);
        Assert.Single(localized.EthicsSection.Principles);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void BuildLocalizations_WhenAnyPartIsOutdated_AggregatedStatusShouldBeOutdated(
        bool sectionOutdated, bool itemOutdated)
    {
        var sectionStatus = sectionOutdated ? TranslationStatus.Outdated : TranslationStatus.Relevant;
        var itemStatus = itemOutdated ? TranslationStatus.Outdated : TranslationStatus.Relevant;

        var source = HippotherapyLandingPageTestDataHelper.CreateFullLocalizationSource(
            sectionStatus: sectionStatus,
            itemStatus: itemStatus);

        var result = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source);

        var localized = Assert.Single(result);
        Assert.Equal(TranslationStatus.Outdated, localized.TranslationStatus);
    }

    [Fact]
    public void BuildStatuses_WhenGivenSource_ShouldCreateEntriesForAllBlocksOrderedCorrectly()
    {
        var source = HippotherapyLandingPageTestDataHelper.CreateFullLocalizationSource(languageId: 1);

        var statuses = HippotherapyLandingPageLocalizationAggregator.BuildStatuses(source);

        Assert.Equal(11, statuses.Count);
        Assert.All(statuses, s => Assert.Equal(1, s.LanguageId));

        var expectedBlocks = Enum.GetValues<HippotherapyLandingPageBlock>().OrderBy(b => b).ToList();
        var actualBlocks = statuses.Select(s => s.Block).ToList();
        Assert.Equal(expectedBlocks, actualBlocks);
    }

    [Fact]
    public void BuildStatuses_WhenAdvantageCardIsOutdated_BlockStatusShouldBeOutdated()
    {
        var source = HippotherapyLandingPageTestDataHelper.CreateFullLocalizationSource(
            sectionStatus: TranslationStatus.Relevant,
            itemStatus: TranslationStatus.Outdated);

        var statuses = HippotherapyLandingPageLocalizationAggregator.BuildStatuses(source);

        var advantagesStatus = statuses.First(s => s.Block == HippotherapyLandingPageBlock.Advantages);
        Assert.Equal(TranslationStatus.Outdated, advantagesStatus.TranslationStatus);
    }
}
