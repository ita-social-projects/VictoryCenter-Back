using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Partners;

public class PartnerProfile : Profile
{
    public PartnerProfile()
    {
        CreateMap<CreatePartnersSectionDto, PartnerSection>();
        CreateMap<UpdatePartnersSectionDto, PartnerSection>();

        CreateMap<CreatePartnerDto, Partner>();
        CreateMap<UpdatePartnerDto, Partner>();

        CreateMap<Partner, PartnerDto>();
        CreateMap<PartnerSection, PartnersSectionDto>();
    }
}
