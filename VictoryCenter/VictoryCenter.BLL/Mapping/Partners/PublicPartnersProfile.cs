using AutoMapper;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Partners;

public class PublicPartnersProfile : Profile
{
    public PublicPartnersProfile()
    {
        CreateMap<PartnerSection, PartnersSectionDto>();
        CreateMap<Partner, PartnerDto>();
        CreateMap<PartnersPageBanner, PartnersPageBannerDto>();

        CreateMap<PartnerSectionLocalization, PartnerSectionLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

        CreateMap<PartnerLocalization, PartnerLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

        CreateMap<PartnersPageBannerLocalization, PartnersPageBannerLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
    }
}
