using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.Enums;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Enums;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.ServiceTests;

public class HippotherapyLandingPageLocalizationAggregatorTests
{
    private static readonly VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto Uk =
        HippotherapyLandingPageTestDataHelper.CreateLanguage(1);

    private static readonly VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto En =
        HippotherapyLandingPageTestDataHelper.CreateLanguage(2);

    [Fact]
    public void BuildLocalizations_WhenSourceIsEmpty_ShouldReturnEmptyList()
    {
        var result = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(
            HippotherapyLandingPageLocalizationSource.Empty);

        Assert.Empty(result);
    }

    [Fact]
    public void BuildStatuses_WhenSourceIsEmpty_ShouldReturnEmptyList()
    {
        var result = HippotherapyLandingPageLocalizationAggregator.BuildStatuses(
            HippotherapyLandingPageLocalizationSource.Empty);

        Assert.Empty(result);
    }

    [Fact]
    public void BuildLocalizations_WhenOnlyIntroAndQuoteTranslated_ShouldLeaveOtherSectionsNull()
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Intro = [Intro(En, "I", TranslationStatus.Relevant)],
            Quote = [Quote(En, "Q", TranslationStatus.Relevant)],
        };

        var localization = Assert.Single(HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source));

        Assert.Equal("I", localization.IntroSection!.Title);
        Assert.Equal("Q", localization.QuoteSection!.QuoteText);
        Assert.Null(localization.DescriptionSection);
        Assert.Null(localization.AdvantagesSection);
        Assert.Null(localization.EthicsSection);
    }

    [Fact]
    public void BuildLocalizations_ShouldReturnLanguagesOrderedAndMergedAcrossSections()
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Intro = [Intro(En, "I", TranslationStatus.Relevant)],
            Quote = [Quote(Uk, "Q", TranslationStatus.Relevant), Quote(En, "Q2", TranslationStatus.Relevant)],
        };

        var result = HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source);

        Assert.Equal([Uk.Id, En.Id], result.Select(l => l.LocalizationInfoDto.Id));
        Assert.Null(result[0].IntroSection);
        Assert.NotNull(result[1].IntroSection);
    }

    [Theory]
    [InlineData(TranslationStatus.Relevant, TranslationStatus.Relevant, TranslationStatus.Relevant)]
    [InlineData(TranslationStatus.Outdated, TranslationStatus.Relevant, TranslationStatus.Outdated)]
    [InlineData(TranslationStatus.Relevant, TranslationStatus.Outdated, TranslationStatus.Outdated)]
    public void BuildLocalizations_LanguageStatus_ShouldBeOutdatedIfAnyBlockOutdated(
        TranslationStatus intro,
        TranslationStatus quote,
        TranslationStatus expected)
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Intro = [Intro(En, "I", intro)],
            Quote = [Quote(En, "Q", quote)],
        };

        var localization = Assert.Single(HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source));

        Assert.Equal(expected, localization.TranslationStatus);
    }

    [Fact]
    public void BuildStatuses_WhenNestedCardIsOutdated_ShouldMarkBlockOutdated()
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Advantages =
            [
                new HippotherapyLandingPageAdvantagesSectionLocalizationDto
                {
                    LocalizationInfoDto = En,
                    TranslationStatus = TranslationStatus.Relevant,
                    Cards =
                    [
                        new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                            { CardId = 1, TranslationStatus = TranslationStatus.Relevant },
                        new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                            { CardId = 2, TranslationStatus = TranslationStatus.Outdated },
                    ],
                },
            ],
        };

        var status = Assert.Single(HippotherapyLandingPageLocalizationAggregator.BuildStatuses(source));

        Assert.Equal(HippotherapyLandingPageBlock.Advantages, status.Block);
        Assert.Equal(TranslationStatus.Outdated, status.TranslationStatus);
    }

    [Fact]
    public void BuildLocalizations_ShouldPreserveEthicsPrinciplesOrderAndIds()
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Ethics =
            [
                new HippotherapyLandingPageEthicsSectionLocalizationDto
                {
                    LocalizationInfoDto = En,
                    Title = "E",
                    Description = "D",
                    Principles =
                    [
                        new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 9, Text = "a" },
                        new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 3, Text = "b" },
                    ],
                },
            ],
        };

        var ethics = Assert.Single(HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source)).EthicsSection!;

        Assert.Equal([9L, 3L], ethics.Principles.Select(p => p.PrincipleId));
        Assert.Equal(["a", "b"], ethics.Principles.Select(p => p.Text));
    }

    [Fact]
    public void BuildStatuses_ShouldReturnOneRecordPerBlockAndLanguageOrderedByBlockThenLanguage()
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Intro = [Intro(En, "I", TranslationStatus.Relevant), Intro(Uk, "I", TranslationStatus.Outdated)],
            Quote = [Quote(En, "Q", TranslationStatus.Relevant)],
        };

        var result = HippotherapyLandingPageLocalizationAggregator.BuildStatuses(source);

        Assert.Equal(
            [
                (HippotherapyLandingPageBlock.Intro, Uk.Id),
                (HippotherapyLandingPageBlock.Intro, En.Id),
                (HippotherapyLandingPageBlock.Quote, En.Id),
            ],
            result.Select(s => (s.Block, s.LanguageId)));
    }

    [Fact]
    public void BuildLocalizations_WhenSectionHasDuplicateRowsForOneLanguage_ShouldUseFirstAndNotThrow()
    {
        var source = HippotherapyLandingPageLocalizationSource.Empty with
        {
            Intro = [Intro(En, "first", TranslationStatus.Relevant), Intro(En, "second", TranslationStatus.Relevant)],
        };

        var localization = Assert.Single(HippotherapyLandingPageLocalizationAggregator.BuildLocalizations(source));

        Assert.Equal("first", localization.IntroSection!.Title);
    }

    private static HippotherapyLandingPageIntroSectionLocalizationDto Intro(
        VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto language,
        string title,
        TranslationStatus status) =>
        new() { LocalizationInfoDto = language, Title = title, Description = "d", TranslationStatus = status };

    private static HippotherapyLandingPageQuoteSectionLocalizationDto Quote(
        VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto language,
        string text,
        TranslationStatus status) =>
        new() { LocalizationInfoDto = language, QuoteText = text, TranslationStatus = status };
}
