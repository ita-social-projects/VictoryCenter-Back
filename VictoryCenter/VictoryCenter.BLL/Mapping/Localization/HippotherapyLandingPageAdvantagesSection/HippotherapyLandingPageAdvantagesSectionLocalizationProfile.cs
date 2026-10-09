using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageAdvantagesSection;

public class HippotherapyLandingPageAdvantagesSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageAdvantagesSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto, HippotherapyLandingPageAdvantagesSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto, HippotherapyLandingPageAdvantagesSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageAdvantagesSectionLocalization, HippotherapyLandingPageAdvantagesSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
