using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class HippotherapyLandingPageHippoventionCenterSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageHippoventionCenterSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto, HippotherapyLandingPageHippoventionCenterSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto, HippotherapyLandingPageHippoventionCenterSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageHippoventionCenterSectionLocalization, HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
