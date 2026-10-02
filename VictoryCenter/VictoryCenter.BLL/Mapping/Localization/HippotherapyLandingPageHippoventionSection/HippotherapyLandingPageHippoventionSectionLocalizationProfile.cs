using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageHippoventionSection;

public class HippotherapyLandingPageHippoventionSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageHippoventionSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageHippoventionSectionLocalizationDto, HippotherapyLandingPageHippoventionSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto, HippotherapyLandingPageHippoventionSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageHippoventionSectionLocalization, HippotherapyLandingPageHippoventionSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
