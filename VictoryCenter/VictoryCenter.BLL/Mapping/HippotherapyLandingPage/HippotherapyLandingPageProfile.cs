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

        CreateMap<HippotherapyLandingPageHippoventionCenterSection, HippoventionCenterSectionDto>();

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

        // Write direction: Update*Dto -> entity. Every map ignores Id/FK/CreatedAt/navigation
        // properties that the handler owns explicitly (fixed-count lists are updated in place by
        // position; the variable-length ScientificReferences collection is diffed by Id) so a plain
        // _mapper.Map(dto, entity) call never touches them.
        CreateMap<UpdateIntroSectionDto, HippotherapyLandingPageIntroSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateTextSectionDto, HippotherapyLandingPageDescriptionSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UpdateTextSectionDto, HippotherapyLandingPageHippoventionSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UpdateTextSectionDto, HippotherapyLandingPageAnalysisSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateQuoteSectionDto, HippotherapyLandingPageQuoteSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UpdateQuoteSectionDto, HippotherapyLandingPageAnotherQuoteSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateHippoventionCenterSectionDto, HippotherapyLandingPageHippoventionCenterSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateGalleryCardDto, HippotherapyLandingPageAdvantageCard>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.AdvantagesSectionId, o => o.Ignore())
            .ForMember(d => d.AdvantagesSection, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.Priority, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UpdateGalleryCardDto, HippotherapyLandingPageParticipantCard>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.ParticipantsSectionId, o => o.Ignore())
            .ForMember(d => d.ParticipantsSection, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.Priority, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateGallerySectionDto, HippotherapyLandingPageAdvantagesSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.AdvantageCards, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UpdateGallerySectionDto, HippotherapyLandingPageParticipantsSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.ParticipantCards, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateScientificReferenceDto, HippotherapyLandingPageScientificReference>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.ScientificReferencesSectionId, o => o.Ignore())
            .ForMember(d => d.ScientificReferencesSection, o => o.Ignore())
            .ForMember(d => d.Priority, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        // ScientificReferences shares its collection property name with the entity (unlike the
        // fixed-count lists above, whose DTO property names never match the entity's), so it must be
        // explicitly ignored here too -- otherwise AutoMapper's convention would try to auto-populate
        // it and clobber the tracked collection the handler diffs by Id.
        CreateMap<UpdateScientificReferencesSectionDto, HippotherapyLandingPageScientificReferencesSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.ScientificReferences, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());

        CreateMap<UpdateEthicsSectionDto, HippotherapyLandingPageEthicsSection>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPageId, o => o.Ignore())
            .ForMember(d => d.HippotherapyLandingPage, o => o.Ignore())
            .ForMember(d => d.Image, o => o.Ignore())
            .ForMember(d => d.EthicsPrinciples, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
    }
}
