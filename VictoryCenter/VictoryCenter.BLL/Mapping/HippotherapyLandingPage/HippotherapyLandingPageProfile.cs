using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.BLL.Mapping.HippotherapyLandingPage;

public class HippotherapyLandingPageProfile : Profile
{
    public HippotherapyLandingPageProfile()
    {
        CreateMap<HippotherapyLandingPageIntroSection, IntroSectionDto>();

        CreateMap<HippotherapyLandingPageDescriptionSection, TextSectionDto>();
        CreateMap<HippotherapyLandingPageHippoventionSection, TextSectionDto>();
        CreateMap<HippotherapyLandingPageAnalysisSection, TextSectionDto>();

        CreateMap<HippotherapyLandingPageQuoteSection, QuoteSectionDto>();
        CreateMap<HippotherapyLandingPageAnotherQuoteSection, QuoteSectionDto>();

        CreateMap<HippotherapyLandingPageHippoventionCenterSection, HippoventionCenterSectionDto>()
            .ForMember(d => d.Pros, o => o.MapFrom(s => s.HippoventionPros.Select(p => p.Text)));

        CreateMap<HippotherapyLandingPageAdvantageCard, GalleryCardDto>();
        CreateMap<HippotherapyLandingPageParticipantCard, GalleryCardDto>();

        CreateMap<HippotherapyLandingPageAdvantagesSection, GallerySectionDto>()
            .ForMember(d => d.Cards, o => o.MapFrom(s => s.AdvantageCards));
        CreateMap<HippotherapyLandingPageParticipantsSection, GallerySectionDto>()
            .ForMember(d => d.Cards, o => o.MapFrom(s => s.ParticipantCards));

        CreateMap<HippotherapyLandingPageScientificReference, ScientificReferenceDto>();
        CreateMap<HippotherapyLandingPageScientificReferencesSection, ScientificReferencesSectionDto>()
            .ForMember(d => d.ScientificReferences, o => o.MapFrom(s => s.ScientificReferences));

        CreateMap<HippotherapyLandingPageEthicsSection, EthicsSectionDto>()
            .ForMember(d => d.Principles, o => o.MapFrom(s => s.EthicsPrinciples.Select(p => p.Text)));

        CreateMap<DAL.Entities.HippotherapyLandingPage, HippotherapyLandingPageDto>();
    }
}
