using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageDescriptionSection;

public class HippotherapyLandingPageDescriptionSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageDescriptionSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageDescriptionSectionLocalizationDto, HippotherapyLandingPageDescriptionSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto, HippotherapyLandingPageDescriptionSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageDescriptionSectionLocalization, HippotherapyLandingPageDescriptionSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
