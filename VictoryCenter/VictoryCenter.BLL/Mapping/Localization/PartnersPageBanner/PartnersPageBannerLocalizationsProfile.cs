using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.PartnersPageBanner;

public class PartnersPageBannerLocalizationsProfile : Profile
{
    public PartnersPageBannerLocalizationsProfile()
    {
        CreateMap<CreatePartnersPageBannerLocalizationDto, PartnersPageBannerLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdatePartnersPageBannerLocalizationDto, PartnersPageBannerLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<PartnersPageBannerLocalization, PartnersPageBannerLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
