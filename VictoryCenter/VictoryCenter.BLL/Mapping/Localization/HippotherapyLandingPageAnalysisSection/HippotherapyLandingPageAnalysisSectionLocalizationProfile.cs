using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.HippotherapyLandingPageAnalysisSection;

public class HippotherapyLandingPageAnalysisSectionLocalizationProfile : Profile
{
    public HippotherapyLandingPageAnalysisSectionLocalizationProfile()
    {
        CreateMap<CreateHippotherapyLandingPageAnalysisSectionLocalizationDto, HippotherapyLandingPageAnalysisSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto, HippotherapyLandingPageAnalysisSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<HippotherapyLandingPageAnalysisSectionLocalization, HippotherapyLandingPageAnalysisSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
