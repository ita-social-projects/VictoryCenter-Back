using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class HippotherapyLandingPageAnotherQuoteSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageAnotherQuoteSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto, HippotherapyLandingPageAnotherQuoteSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto, HippotherapyLandingPageAnotherQuoteSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageAnotherQuoteSectionLocalization, HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
