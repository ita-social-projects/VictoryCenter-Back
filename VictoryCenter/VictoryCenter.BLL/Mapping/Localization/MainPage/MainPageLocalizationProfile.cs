using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.MainPage;

public class MainPageLocalizationProfile : Profile
{
    public MainPageLocalizationProfile()
    {
        CreateMap<CreateMainPageLocalizationDto, MainPageLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateMainPageLocalizationDto, MainPageLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<MainPageLocalization, MainPageLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.MainAboutUs, opt => opt.Ignore())
            .ForMember(dest => dest.MainPartners, opt => opt.Ignore())
            .ForMember(dest => dest.MainDonations, opt => opt.Ignore());

        CreateMap<CreateMainAboutUsLocalizationDto, MainAboutUsLocalization>()
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateMainAboutUsLocalizationDto, MainAboutUsLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<MainAboutUsLocalization, MainAboutUsLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));

        CreateMap<CreateMainPartnersLocalizationDto, MainPartnersLocalization>()
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateMainPartnersLocalizationDto, MainPartnersLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<MainPartnersLocalization, MainPartnersLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));

        CreateMap<CreateMainDonationsLocalizationDto, MainDonationsLocalization>()
            .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateMainDonationsLocalizationDto, MainDonationsLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<MainDonationsLocalization, MainDonationsLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
