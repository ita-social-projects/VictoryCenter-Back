using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageIntroSection;

public class HippotherapyLandingPageIntroSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageIntroSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageIntroSectionLocalizationDto, HippotherapyLandingPageIntroSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageIntroSectionLocalizationDto, HippotherapyLandingPageIntroSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageIntroSectionLocalization, HippotherapyLandingPageIntroSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
