using AutoMapper;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Partners;

public class PublicPartnersProfile : Profile
{
    public PublicPartnersProfile()
    {
        CreateMap<PartnerSection, PublicPartnersSectionDto>();
        CreateMap<Partner, PublicPartnerDto>();
        CreateMap<PartnersPageBanner, PublicPartnersPageBannerDto>();

        CreateMap<PartnerSectionLocalization, PublicPartnerSectionLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

        CreateMap<PartnerLocalization, PublicPartnerLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

        CreateMap<PartnersPageBannerLocalization, PublicPartnersPageBannerLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
    }
}
