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
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

public static class HippotherapyLandingPageTestDataHelper
{
    public static LocalizationInfoDto CreateLanguage(long id = 1, string code = "en") => new()
    {
        Id = id,
        Code = code,
    };

    public static HippotherapyLandingPage CreateLandingPageEntity(long baseId = 1) => new()
    {
        Id = baseId,
        IntroSection = new HippotherapyLandingPageIntroSection { Id = baseId + 1 },
        DescriptionSection = new HippotherapyLandingPageDescriptionSection { Id = baseId + 2 },
        QuoteSection = new HippotherapyLandingPageQuoteSection { Id = baseId + 3 },
        HippoventionSection = new HippotherapyLandingPageHippoventionSection { Id = baseId + 4 },
        HippoventionCenterSection = new HippotherapyLandingPageHippoventionCenterSection { Id = baseId + 5 },
        AdvantagesSection = new HippotherapyLandingPageAdvantagesSection { Id = baseId + 6 },
        AnalysisSection = new HippotherapyLandingPageAnalysisSection { Id = baseId + 7 },
        ScientificReferencesSection = new HippotherapyLandingPageScientificReferencesSection { Id = baseId + 8 },
        AnotherQuoteSection = new HippotherapyLandingPageAnotherQuoteSection { Id = baseId + 9 },
        ParticipantsSection = new HippotherapyLandingPageParticipantsSection { Id = baseId + 10 },
        EthicsSection = new HippotherapyLandingPageEthicsSection { Id = baseId + 11 },
    };

    public static HippotherapyLandingPageLocalizationSource CreateFullLocalizationSource(
        long languageId = 1,
        TranslationStatus sectionStatus = TranslationStatus.Relevant,
        TranslationStatus? itemStatus = TranslationStatus.Relevant)
    {
        var lang = CreateLanguage(languageId);

        return new HippotherapyLandingPageLocalizationSource(
            Intro: [new HippotherapyLandingPageIntroSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Intro", Description = "Desc", TranslationStatus = sectionStatus }],
            Description: [new HippotherapyLandingPageDescriptionSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Desc Title", Description = "Desc", TranslationStatus = sectionStatus }],
            Quote: [new HippotherapyLandingPageQuoteSectionLocalizationDto { LocalizationInfoDto = lang, QuoteText = "Quote", AuthorName = "Author", TranslationStatus = sectionStatus }],
            Hippovention: [new HippotherapyLandingPageHippoventionSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Hippo Title", Description = "Desc", TranslationStatus = sectionStatus }],
            HippoventionCenter: [new HippotherapyLandingPageHippoventionCenterSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Center", Description = "Desc", Pros = "Pros", TranslationStatus = sectionStatus }],
            Advantages: [new HippotherapyLandingPageAdvantagesSectionLocalizationDto
            {
                LocalizationInfoDto = lang,
                Title = "Adv Title",
                TranslationStatus = sectionStatus,
                Cards = [new HippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 1, Description = "Card 1", TranslationStatus = itemStatus }],
            }

            ],
            Analysis: [new HippotherapyLandingPageAnalysisSectionLocalizationDto { LocalizationInfoDto = lang, Title = "Analysis", Description = "Desc", TranslationStatus = sectionStatus }],
            ScientificReferences: [new HippotherapyLandingPageScientificReferencesSectionLocalizationDto
            {
                LocalizationInfoDto = lang,
                Title = "Sci Title",
                Description = "Desc",
                TranslationStatus = sectionStatus,
                References = [new HippotherapyLandingPageScientificReferenceLocalizationItemDto { ReferenceId = 1, Name = "Ref 1", TranslationStatus = itemStatus }],
            }

            ],
            AnotherQuote: [new HippotherapyLandingPageAnotherQuoteSectionLocalizationDto { LocalizationInfoDto = lang, QuoteText = "Another Quote", AuthorName = "Author", TranslationStatus = sectionStatus }],
            Participants: [new HippotherapyLandingPageParticipantsSectionLocalizationDto
            {
                LocalizationInfoDto = lang,
                Title = "Part Title",
                TranslationStatus = sectionStatus,
                Cards = [new HippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 1, Description = "Part 1", TranslationStatus = itemStatus }],
            }

            ],
            Ethics: [new HippotherapyLandingPageEthicsSectionLocalizationDto
            {
                LocalizationInfoDto = lang,
                Title = "Ethics Title",
                Description = "Desc",
                TranslationStatus = sectionStatus,
                Principles = [new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 1, Text = "Principle 1", TranslationStatus = itemStatus }],
            }

            ]);
    }
}
