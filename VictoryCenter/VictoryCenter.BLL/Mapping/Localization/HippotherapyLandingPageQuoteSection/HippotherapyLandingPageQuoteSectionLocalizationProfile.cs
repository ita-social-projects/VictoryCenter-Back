using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageQuoteSection;

public class HippotherapyLandingPageQuoteSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageQuoteSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageQuoteSectionLocalizationDto, HippotherapyLandingPageQuoteSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageQuoteSectionLocalizationDto, HippotherapyLandingPageQuoteSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageQuoteSectionLocalization, HippotherapyLandingPageQuoteSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
