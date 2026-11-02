using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.Enums;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Enums;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.ServiceTests;

public class HippotherapyLandingPageResponseFactoryTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IHippotherapyLandingPageLocalizationBuilder> _builderMock = new();

    [Fact]
    public void Create_ShouldKeepMappedSectionsAndAddLocalizationsAndStatuses()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var mapped = CreateEmptyDto();
        var language = HippotherapyLandingPageTestDataHelper.CreateLanguage(2);
        _mapperMock.Setup(m => m.Map<HippotherapyLandingPageDto>(page)).Returns(mapped);
        _builderMock.Setup(b => b.Build(page)).Returns(SourceWithIntro(language, TranslationStatus.Relevant));

        var result = CreateFactory().Create(page);

        var localization = Assert.Single(result.Localizations);
        Assert.Equal(language.Id, localization.LocalizationInfoDto.Id);
        Assert.Equal("Intro EN", localization.IntroSection!.Title);
        var status = Assert.Single(result.TranslationStatuses);
        Assert.Equal(HippotherapyLandingPageBlock.Intro, status.Block);
        Assert.Equal(language.Id, status.LanguageId);
        Assert.Equal(TranslationStatus.Relevant, status.TranslationStatus);
    }

    [Fact]
    public void Create_ShouldNotMutateMappedDto()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        var mapped = CreateEmptyDto();
        _mapperMock.Setup(m => m.Map<HippotherapyLandingPageDto>(page)).Returns(mapped);
        _builderMock.Setup(b => b.Build(page)).Returns(
            SourceWithIntro(HippotherapyLandingPageTestDataHelper.CreateLanguage(2), TranslationStatus.Relevant));

        var result = CreateFactory().Create(page);

        Assert.NotSame(mapped, result);
        Assert.Empty(mapped.Localizations);
        Assert.Empty(mapped.TranslationStatuses);
    }

    [Fact]
    public void Create_WhenNoTranslationsExist_ShouldReturnEmptyCollections()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        _mapperMock.Setup(m => m.Map<HippotherapyLandingPageDto>(page)).Returns(CreateEmptyDto());
        _builderMock.Setup(b => b.Build(page)).Returns(HippotherapyLandingPageLocalizationSource.Empty);

        var result = CreateFactory().Create(page);

        Assert.Empty(result.Localizations);
        Assert.Empty(result.TranslationStatuses);
    }

    [Fact]
    public void Create_ShouldBuildLocalizationSourceOnlyOnce()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        _mapperMock.Setup(m => m.Map<HippotherapyLandingPageDto>(page)).Returns(CreateEmptyDto());
        _builderMock.Setup(b => b.Build(page)).Returns(HippotherapyLandingPageLocalizationSource.Empty);

        CreateFactory().Create(page);

        _builderMock.Verify(b => b.Build(page), Times.Once);
    }

    private HippotherapyLandingPageResponseFactory CreateFactory() =>
        new(_mapperMock.Object, _builderMock.Object);

    private static HippotherapyLandingPageLocalizationSource SourceWithIntro(
        VictoryCenter.BLL.DTOs.Common.LocalizationInfoDto language,
        TranslationStatus status) =>
        HippotherapyLandingPageLocalizationSource.Empty with
        {
            Intro =
            [
                new HippotherapyLandingPageIntroSectionLocalizationDto
                {
                    LocalizationInfoDto = language,
                    Title = "Intro EN",
                    Description = "Desc EN",
                    TranslationStatus = status,
                },
            ],
        };

    private static HippotherapyLandingPageDto CreateEmptyDto() => new()
    {
        IntroSection = null!,
        DescriptionSection = null!,
        QuoteSection = null!,
        HippoventionSection = null!,
        HippoventionCenterSection = null!,
        AdvantagesSection = null!,
        AnalysisSection = null!,
        ScientificReferencesSection = null!,
        AnotherQuoteSection = null!,
        ParticipantsSection = null!,
        EthicsSection = null!,
    };
}
