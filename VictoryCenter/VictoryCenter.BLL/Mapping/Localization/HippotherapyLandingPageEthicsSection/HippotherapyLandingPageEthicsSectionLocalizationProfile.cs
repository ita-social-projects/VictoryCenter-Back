using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageEthicsSection;

public class HippotherapyLandingPageEthicsSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageEthicsSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageEthicsSectionLocalizationDto, HippotherapyLandingPageEthicsSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageEthicsSectionLocalizationDto, HippotherapyLandingPageEthicsSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageEthicsSectionLocalization, HippotherapyLandingPageEthicsSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.Principles, opt => opt.Ignore());

        CreateMap<UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto, HippotherapyLandingPageEthicsPrincipleLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.PrincipleId))
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<HippotherapyLandingPageEthicsPrincipleLocalization, HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>()
            .ForMember(dest => dest.PrincipleId, opt => opt.MapFrom(src => src.EntityId));
    }
}
