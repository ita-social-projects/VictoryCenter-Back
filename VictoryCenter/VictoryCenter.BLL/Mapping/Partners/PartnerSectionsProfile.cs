using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Partners;

public class PartnerProfile : Profile
{
    public PartnerProfile()
    {
        CreateMap<CreatePartnerDto, Partner>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<CreatePartnersSectionDto, PartnerSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdatePartnerDto, Partner>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdatePartnersSectionDto, PartnerSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdatePartnersPageBannerDto, PartnersPageBanner>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<Partner, PartnerDto>();
        CreateMap<PartnerSection, PartnersSectionDto>();
        CreateMap<PartnersPageBanner, PartnersPageBannerDto>();
    }
}
