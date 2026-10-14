using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageScientificReferencesSection;

public class HippotherapyLandingPageScientificReferencesSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageScientificReferencesSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto, HippotherapyLandingPageScientificReferencesSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto, HippotherapyLandingPageScientificReferencesSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageScientificReferencesSectionLocalization, HippotherapyLandingPageScientificReferencesSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.References, opt => opt.Ignore());

        CreateMap<UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto, HippotherapyLandingPageScientificReferenceLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.ReferenceId))
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<HippotherapyLandingPageScientificReferenceLocalization, HippotherapyLandingPageScientificReferenceLocalizationItemDto>()
            .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.EntityId));
    }
}
